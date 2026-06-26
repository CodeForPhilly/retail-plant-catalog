namespace webapi.Services;

using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Shared;

/// <summary>
/// Singleton implementation of <see cref="ICrawlQueue"/>. Persists to <c>crawl_job</c>
/// (via a scoped <see cref="CrawlJobRepository"/>) and emits a wake-up signal on an
/// in-memory bounded channel so any waiting worker in <see cref="CrawlWorkerService"/>
/// resumes immediately.
///
/// <para>The signal channel uses <see cref="BoundedChannelFullMode.DropWrite"/> with a
/// small capacity — the channel only carries wake-up notifications, not job data, so
/// dropping duplicate signals is fine. Workers always re-check the DB on wake-up via
/// <see cref="CrawlJobRepository.TryClaimNextQueued"/>.</para>
/// </summary>
public class CrawlQueue : ICrawlQueue
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<byte> _wakeup;

    public CrawlQueue(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        // Capacity 2 + DropWrite: any further "wake up" signals while the channel is full
        // are silently discarded. Workers re-check the DB on each wake-up so missing a
        // duplicate signal is harmless.
        _wakeup = Channel.CreateBounded<byte>(new BoundedChannelOptions(capacity: 2)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = false,
            SingleWriter = false,
        });
    }

    /// <summary>Reader used by <see cref="CrawlWorkerService"/> to wait for wake-up signals.</summary>
    internal ChannelReader<byte> WakeupReader => _wakeup.Reader;

    public async Task<string> EnqueueAsync(CrawlJob job, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(job);
        if (string.IsNullOrWhiteSpace(job.VendorId))
            throw new ArgumentException("VendorId is required on a crawl job.", nameof(job));

        using var scope = _serviceProvider.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<CrawlJobRepository>();

        // Best-effort coalescing: if this vendor already has an open full-vendor job,
        // return its id instead of creating a duplicate. Single-URL jobs are not
        // coalesced (different URLs are independent operations).
        //
        // This is best-effort because two concurrent EnqueueAsync calls can both pass
        // this check before either INSERT commits. The worker's vendor-lock fallback
        // (see CrawlWorkerService.ExecuteJobAsync) handles that race by marking
        // the duplicate job Cancelled when it tries to run.
        if (string.IsNullOrEmpty(job.UrlId))
        {
            var existing = repo.GetOpenJobForVendor(job.VendorId);
            if (existing != null && string.IsNullOrEmpty(existing.UrlId))
            {
                _ = _wakeup.Writer.TryWrite(0);
                return existing.Id;
            }
        }

        var id = repo.Enqueue(job);

        // Best-effort signal. The channel is non-blocking when full (DropWrite).
        _ = _wakeup.Writer.TryWrite(0);
        await Task.CompletedTask;
        return id;
    }

    public int QueueDepth
    {
        get
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<CrawlJobRepository>();
            return repo.CountQueued();
        }
    }
}
