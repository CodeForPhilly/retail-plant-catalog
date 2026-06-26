namespace webapi.Services;

using FluentLogger.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Shared;

/// <summary>
/// Long-running hosted service that drains the <c>crawl_job</c> queue with a
/// fixed-size worker pool. Each worker claims the next queued job via
/// <see cref="CrawlJobRepository.TryClaimNextQueued"/> (atomic SELECT FOR UPDATE +
/// status transition) and executes the crawl in its own DI scope.
///
/// <para>Pool size is configured via <c>Crawl:MaxConcurrentWorkers</c> in
/// appsettings (default 3). Idle workers wait on the in-memory wake-up channel
/// from <see cref="CrawlQueue"/>, with a periodic poll fallback (configured by
/// <c>Crawl:QueuePollIntervalSeconds</c>, default 30) to guarantee progress
/// even if a wake-up signal was missed (e.g. jobs inserted directly via SQL
/// during diagnostic work).</para>
/// </summary>
public class CrawlWorkerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICrawlQueue _queue;
    private readonly IConfiguration _configuration;
    private readonly ILog _logger;

    public CrawlWorkerService(
        IServiceProvider serviceProvider,
        ICrawlQueue queue,
        IConfiguration configuration,
        ILog logger)
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var maxWorkers = Math.Max(1, _configuration.GetValue<int?>("Crawl:MaxConcurrentWorkers") ?? 3);
        var pollSeconds = Math.Max(5, _configuration.GetValue<int?>("Crawl:QueuePollIntervalSeconds") ?? 30);
        var reapThresholdMin = Math.Max(15, _configuration.GetValue<int?>("Crawl:StuckRunningThresholdMinutes") ?? 60);
        var janitorIntervalMin = Math.Max(1, _configuration.GetValue<int?>("Crawl:JanitorIntervalMinutes") ?? 15);
        var staleThreshold = TimeSpan.FromMinutes(reapThresholdMin);
        var janitorInterval = TimeSpan.FromMinutes(janitorIntervalMin);

        _logger.Info(
            $"CrawlWorkerService: starting {maxWorkers} worker(s). Poll fallback: {pollSeconds}s. " +
            $"Janitor: every {janitorIntervalMin}m, reap Running > {reapThresholdMin}m.");

        // Startup reap: any Running rows in the table are owned by no live worker (the prior
        // process is gone by the time this BackgroundService starts). Clean them up before new
        // workers begin claiming jobs so the scheduler stops skipping their vendors.
        ReapStuckJobsAndClearFlags(staleThreshold, "startup");

        var workers = new List<Task>(maxWorkers + 1);
        for (int i = 0; i < maxWorkers; i++)
        {
            int workerId = i + 1;
            workers.Add(Task.Run(() => WorkerLoop(workerId, pollSeconds, stoppingToken), stoppingToken));
        }
        // Janitor: defense-in-depth periodic sweep for jobs that leak between restarts.
        workers.Add(Task.Run(() => JanitorLoop(staleThreshold, janitorInterval, stoppingToken), stoppingToken));

        try
        {
            await Task.WhenAll(workers);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // expected on shutdown
        }
        catch (Exception ex)
        {
            _logger.Error("CrawlWorkerService: fatal error from worker tasks.", ex);
        }
        finally
        {
            _logger.Info("CrawlWorkerService: all workers stopped.");
        }
    }

    private async Task WorkerLoop(int workerId, int pollSeconds, CancellationToken stoppingToken)
    {
        _logger.Info($"CrawlWorkerService: worker {workerId} started.");
        var wakeupReader = (_queue as CrawlQueue)?.WakeupReader;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Drain everything currently queued before going back to wait.
                while (!stoppingToken.IsCancellationRequested)
                {
                    var claimed = TryClaimJob();
                    if (claimed == null) break;
                    await ExecuteJobAsync(workerId, claimed, stoppingToken);
                }

                // No work right now. Wait for wake-up or poll timeout, whichever first.
                if (stoppingToken.IsCancellationRequested) break;
                if (wakeupReader != null)
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    cts.CancelAfter(TimeSpan.FromSeconds(pollSeconds));
                    try
                    {
                        await wakeupReader.ReadAsync(cts.Token);
                    }
                    catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
                    {
                        // poll timeout - fall through and re-check the DB
                    }
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(pollSeconds), stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.Error($"CrawlWorkerService: worker {workerId} loop error; backing off 10s.", ex);
                try { await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); }
                catch (OperationCanceledException) { break; }
            }
        }

        _logger.Info($"CrawlWorkerService: worker {workerId} stopped.");
    }

    private CrawlJob? TryClaimJob()
    {
        using var scope = _serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<CrawlJobRepository>();
        return repo.TryClaimNextQueued();
    }

    /// <summary>
    /// Periodically sweeps the crawl_job table for rows orphaned in <c>Running</c> state and
    /// clears the corresponding stale <c>vendor.CrawlInProgress</c> flags. Catches the case
    /// where the host process is killed between MarkDone/MarkFailed and the finally block —
    /// most commonly a systemd shutdown that exceeds the configured timeout, or a hard crash.
    /// </summary>
    private async Task JanitorLoop(TimeSpan staleThreshold, TimeSpan interval, CancellationToken stoppingToken)
    {
        _logger.Info("CrawlWorkerService: janitor started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await Task.Delay(interval, stoppingToken); }
            catch (OperationCanceledException) { break; }
            try
            {
                ReapStuckJobsAndClearFlags(staleThreshold, "janitor");
            }
            catch (Exception ex)
            {
                _logger.Error("CrawlWorkerService: janitor sweep failed.", ex);
            }
        }
        _logger.Info("CrawlWorkerService: janitor stopped.");
    }

    private void ReapStuckJobsAndClearFlags(TimeSpan staleThreshold, string trigger)
    {
        using var scope = _serviceProvider.CreateScope();
        var jobRepo = scope.ServiceProvider.GetRequiredService<CrawlJobRepository>();
        var vendorRepo = scope.ServiceProvider.GetRequiredService<VendorRepository>();
        var reaped = jobRepo.ReapStuckRunning(staleThreshold);
        var cleared = vendorRepo.ClearOrphanedCrawlInProgress();
        if (reaped > 0 || cleared > 0)
        {
            _logger.Info(
                $"CrawlWorkerService: {trigger} sweep reaped {reaped} stuck-Running job(s) " +
                $"and cleared {cleared} orphaned vendor.CrawlInProgress flag(s).");
        }
    }

    private async Task ExecuteJobAsync(int workerId, CrawlJob job, CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<CrawlJobRepository>();
        var crawler = scope.ServiceProvider.GetRequiredService<PlantCrawler>();
        var vendorService = scope.ServiceProvider.GetRequiredService<VendorService>();
        var vendorRepo = scope.ServiceProvider.GetRequiredService<VendorRepository>();

        bool fullVendorLockClaimed = false;
        try
        {
            _logger.Info($"CrawlWorkerService: worker {workerId} starting job {job.Id} " +
                         $"(Vendor={job.VendorId}, UrlId={job.UrlId ?? "<full>"} , Source={job.Source}).");
            crawler.Init();

            if (!string.IsNullOrEmpty(job.UrlId))
            {
                // Single-URL path: PlantCrawler.CrawlSingleUrl handles its own URL-level
                // atomic claim. No vendor-level lock needed.
                var ran = await crawler.CrawlSingleUrl(job.VendorId, job.UrlId);
                if (!ran)
                {
                    repo.MarkFailed(job.Id, "URL not found or lock not claimed");
                    _logger.Warn($"CrawlWorkerService: job {job.Id} could not claim URL lock; marked Failed.");
                    return;
                }
            }
            else
            {
                var vendor = vendorService.GetPopulatedVendor(job.VendorId);
                if (vendor == null)
                {
                    repo.MarkFailed(job.Id, "Vendor not found");
                    _logger.Warn($"CrawlWorkerService: job {job.Id} vendor not found; marked Failed.");
                    return;
                }

                // Full-vendor path: claim the vendor-level atomic lock before
                // running the crawl. Coalescing at enqueue is best-effort - if two
                // concurrent EnqueueAsync calls both passed the GetOpenJobForVendor
                // check before either INSERT committed, we end up with duplicate jobs
                // for the same vendor. This lock ensures only ONE of those jobs
                // actually runs the crawl; the rest get marked Cancelled and the
                // caller's polling will find the active sibling job via GetOpenJobForVendor.
                if (!vendorRepo.TryMarkCrawlInProgress(job.VendorId))
                {
                    repo.MarkCancelled(job.Id);
                    _logger.Info($"CrawlWorkerService: job {job.Id} coalesced with another open " +
                                 $"crawl for vendor {job.VendorId}; marked Cancelled.");
                    return;
                }
                fullVendorLockClaimed = true;

                await crawler.Crawl(vendor);
                // Crawl() sets vendor.LastCrawled and vendor.PlantCount from the real association
                // count (so a failed/empty crawl can't zero a good catalog). Here we only mirror the
                // per-URL-derived counters the controller paths set.
                vendor.CrawlErrors = vendor.PlantListingUris?
                    .Count(u => u.LastStatus != CrawlStatus.None && u.LastStatus != CrawlStatus.Ok) ?? 0;
                if (vendor.PlantListingUris != null && vendor.PlantListingUris.Length > 0)
                {
                    var mostRecent = vendor.PlantListingUris
                        .Select(u => new { u.LastStatus, Time = u.LastSucceeded ?? u.LastFailed })
                        .Where(x => x.Time != null)
                        .OrderByDescending(x => x.Time)
                        .FirstOrDefault();
                    vendor.LastCrawlStatus = mostRecent?.LastStatus ?? CrawlStatus.None;
                }
                vendorRepo.Update(vendor);
            }

            repo.MarkDone(job.Id);
            _logger.Info($"CrawlWorkerService: worker {workerId} completed job {job.Id}.");
        }
        catch (Exception ex)
        {
            var msg = ex.Message?.Length > 1000 ? ex.Message[..1000] : ex.Message ?? string.Empty;
            repo.MarkFailed(job.Id, msg);
            _logger.Error($"CrawlWorkerService: worker {workerId} failed job {job.Id}.", ex);
        }
        finally
        {
            if (fullVendorLockClaimed)
            {
                try { vendorRepo.ClearCrawlInProgress(job.VendorId); }
                catch (Exception ex)
                {
                    _logger.Error($"CrawlWorkerService: worker {workerId} failed to clear " +
                                  $"CrawlInProgress for vendor {job.VendorId}.", ex);
                }
            }
        }
    }
}
