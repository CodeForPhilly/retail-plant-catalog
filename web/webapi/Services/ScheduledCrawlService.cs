using System.Globalization;
using FluentLogger.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Shared;
using webapi.Services;

namespace webapi.Services;

/// <summary>
/// Background service that runs a nightly crawl for vendors with uncrawled or stale plant listing URLs.
/// Schedule and concurrency are configured via ScheduledCrawl in appsettings.
/// </summary>
public class ScheduledCrawlService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILog _logger;
    private DateTime? _lastRunDate;

    /// <summary>Creates the scheduled crawl background service.</summary>
    public ScheduledCrawlService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILog logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_configuration.GetValue<bool>("ScheduledCrawl:Enabled"))
        {
            _logger.Info("ScheduledCrawlService: Scheduled crawl is disabled. Set ScheduledCrawl:Enabled to true to run nightly crawls.");
            return;
        }

        var timeStr = _configuration.GetValue<string>("ScheduledCrawl:Time") ?? "02:00";
        if (!TimeSpan.TryParseExact(timeStr, "hh\\:mm", CultureInfo.InvariantCulture, out var scheduledTime))
        {
            _logger.Warn($"ScheduledCrawlService: Invalid ScheduledCrawl:Time '{timeStr}'. Use HH:mm (e.g. 02:00). Defaulting to 02:00.");
            scheduledTime = new TimeSpan(2, 0, 0);
        }

        _logger.Info($"ScheduledCrawlService: Started. Will run daily at {scheduledTime:hh\\:mm} for vendors with uncrawled or stale URLs.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                var today = now.Date;
                var currentTime = now.TimeOfDay;
                // Run if we're within the same minute as scheduled time and haven't run today
                var scheduledTimeOfDay = new TimeSpan(scheduledTime.Hours, scheduledTime.Minutes, 0);
                var oneMinute = TimeSpan.FromMinutes(1);
                var inWindow = currentTime >= scheduledTimeOfDay && currentTime < scheduledTimeOfDay.Add(oneMinute);
                var notRunToday = _lastRunDate != today;

                if (inWindow && notRunToday)
                {
                    _lastRunDate = today;
                    await RunScheduledCrawlAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.Error("ScheduledCrawlService: Error in schedule check loop.", ex);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task RunScheduledCrawlAsync(CancellationToken stoppingToken)
    {
        // Enqueue rather than crawl inline. The actual crawls run on the
        // CrawlWorkerService pool at MaxConcurrentWorkers rate, which gives us a
        // hard ceiling on simultaneous crawls and lets the queue act as backpressure.
        // This loop now finishes in seconds (just inserts rows); the workers chew
        // through the queue at their own pace over the rest of the night.
        _logger.Info("ScheduledCrawlService: Starting nightly enqueue for uncrawled URLs.");

        using var scope = _serviceProvider.CreateScope();
        var vendorRepository = scope.ServiceProvider.GetRequiredService<VendorRepository>();
        var vendorService = scope.ServiceProvider.GetRequiredService<VendorService>();
        var crawlQueue = scope.ServiceProvider.GetRequiredService<ICrawlQueue>();

        IEnumerable<string> vendorIds;
        try
        {
            vendorIds = vendorRepository.GetVendorIdsWithUncrawledUrls().ToList();
        }
        catch (Exception ex)
        {
            _logger.Error("ScheduledCrawlService: Failed to query vendors with uncrawled URLs.", ex);
            return;
        }

        var total = vendorIds.Count();
        if (total == 0)
        {
            _logger.Info("ScheduledCrawlService: No vendors with uncrawled or stale URLs. Skipping.");
            return;
        }

        _logger.Info($"ScheduledCrawlService: Found {total} vendor(s) to enqueue.");

        var enqueued = 0;
        var skipped = 0;
        var errors = 0;

        foreach (var vendorId in vendorIds)
        {
            if (stoppingToken.IsCancellationRequested) break;

            try
            {
                var populatedVendor = vendorService.GetPopulatedVendor(vendorId);
                if (populatedVendor?.Id == null || populatedVendor.PlantListingUris == null || !populatedVendor.PlantListingUris.Any())
                {
                    skipped++;
                    continue;
                }

                var jobId = await crawlQueue.EnqueueAsync(new CrawlJob
                {
                    VendorId = populatedVendor.Id,
                    Source = CrawlJobSource.Schedule,
                }, stoppingToken);
                enqueued++;
            }
            catch (Exception ex)
            {
                errors++;
                _logger.Error($"ScheduledCrawlService: Error enqueuing crawl for vendor {vendorId}. Continuing.", ex);
            }
        }

        _logger.Info(
            $"ScheduledCrawlService: Nightly enqueue complete. Enqueued={enqueued}, Skipped={skipped}, " +
            $"Errors={errors}, Total={total}. Workers will process the queue at MaxConcurrentWorkers rate.");
    }
}
