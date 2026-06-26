namespace webapi.Services;

using Shared;

/// <summary>
/// Application-facing queue for vendor crawls. Enqueue persists a row to the
/// <c>crawl_job</c> table and signals any idle worker in <see cref="CrawlWorkerService"/>
/// to pick it up. The DB table is the source of truth; the in-memory signal channel
/// is just for low-latency wake-up.
///
/// </summary>
public interface ICrawlQueue
{
    /// <summary>
    /// Persist a job in <see cref="CrawlJobStatus.Queued"/> state and signal the worker
    /// pool. Returns the assigned job id (caller may supply one in <see cref="CrawlJob.Id"/>
    /// for idempotency, or leave it null for auto-generation).
    /// </summary>
    Task<string> EnqueueAsync(CrawlJob job, CancellationToken cancellationToken = default);

    /// <summary>Current rows in <see cref="CrawlJobStatus.Queued"/> state.</summary>
    int QueueDepth { get; }
}
