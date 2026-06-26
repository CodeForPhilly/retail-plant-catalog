using Dapper.Contrib.Extensions;

namespace Shared;

/// <summary>
/// Lifecycle status of a queued crawl job. Persisted as a MySQL ENUM string;
/// see <c>Repositories.CrawlJobStatusTypeHandler</c> for Dapper mapping.
/// </summary>
public enum CrawlJobStatus
{
    Queued = 0,
    Running = 1,
    Done = 2,
    Failed = 3,
    Cancelled = 4,
}

/// <summary>
/// Where a crawl job originated. Used for observability and to inform retry/coalesce
/// policy. Persisted as a MySQL ENUM string; see <c>Repositories.CrawlJobSourceTypeHandler</c>.
/// </summary>
public enum CrawlJobSource
{
    UI = 0,
    MCP = 1,
    Schedule = 2,
    Bulk = 3,
    Other = 4,
}

/// <summary>
/// A queued or running crawl job. Created by enqueue-style controller endpoints
/// and the scheduled-crawl service; consumed by <c>CrawlWorkerService</c>.
///
/// <para>If <see cref="UrlId"/> is null the worker crawls all of the vendor's
/// PlantListingUris; if set, only that one URL is crawled.</para>
/// </summary>
[Table("crawl_job")]
public class CrawlJob
{
    [ExplicitKey]
    public string Id { get; set; } = null!;

    /// <summary>Vendor being crawled. Required.</summary>
    public string VendorId { get; set; } = null!;

    /// <summary>Optional single-URL target. Null = full-vendor crawl.</summary>
    public string? UrlId { get; set; }

    public CrawlJobStatus Status { get; set; } = CrawlJobStatus.Queued;

    public CrawlJobSource Source { get; set; } = CrawlJobSource.Other;

    /// <summary>User who enqueued this job, if known. Admin id for UI; null for Schedule.</summary>
    public string? RequestedBy { get; set; }

    public DateTime EnqueuedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    /// <summary>Failure detail when <see cref="Status"/> is <see cref="CrawlJobStatus.Failed"/>.</summary>
    public string? Error { get; set; }
}
