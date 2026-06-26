namespace Repositories;

using Dapper;
using Shared;
using System.Data;

/// <summary>
/// Persisted FIFO queue of <see cref="CrawlJob"/> rows. The crawl worker calls
/// <see cref="TryClaimNextQueued"/> to atomically pick up the oldest queued job;
/// terminal transitions go through <see cref="MarkDone"/>, <see cref="MarkFailed"/>,
/// or <see cref="MarkCancelled"/>.
/// </summary>
public class CrawlJobRepository : Repository<CrawlJob>
{
    public CrawlJobRepository(IDbConnection connection) : base(connection)
    {
    }

    /// <summary>
    /// Insert a fresh job in <see cref="CrawlJobStatus.Queued"/> state. The caller may
    /// supply an Id (deterministic / external) or leave it null/empty for an auto-generated GUID.
    ///
    /// <para>Uses <see cref="DynamicParameters"/> with explicit <c>DbType.String</c> for
    /// the enum columns. Dapper's enum handling for INSERTs sends the underlying int
    /// (0,1,...) by default even with a TypeHandler registered, which MySQL rejects
    /// against the ENUM('Queued',...) column with "Data truncated". Same pattern is used
    /// by <see cref="VendorUrlRepository"/> for its LastStatus enum.</para>
    /// </summary>
    public string Enqueue(CrawlJob job)
    {
        if (string.IsNullOrEmpty(job.Id))
            job.Id = Guid.NewGuid().ToString();
        if (job.EnqueuedAt == default)
            job.EnqueuedAt = DateTime.UtcNow;
        if (job.Status == default)
            job.Status = CrawlJobStatus.Queued;
        var parameters = new DynamicParameters();
        parameters.Add("@Id", job.Id);
        parameters.Add("@VendorId", job.VendorId);
        parameters.Add("@UrlId", job.UrlId);
        parameters.Add("@Status", job.Status.ToString(), System.Data.DbType.String);
        parameters.Add("@Source", job.Source.ToString(), System.Data.DbType.String);
        parameters.Add("@RequestedBy", job.RequestedBy);
        parameters.Add("@EnqueuedAt", job.EnqueuedAt);
        conn.Execute(
            @"INSERT INTO crawl_job (Id, VendorId, UrlId, Status, Source, RequestedBy, EnqueuedAt)
              VALUES (@Id, @VendorId, @UrlId, @Status, @Source, @RequestedBy, @EnqueuedAt)",
            parameters);
        return job.Id;
    }

    /// <summary>
    /// Atomically pick up the oldest <see cref="CrawlJobStatus.Queued"/> job and transition it
    /// to <see cref="CrawlJobStatus.Running"/> with <see cref="CrawlJob.StartedAt"/> set.
    /// Returns the claimed job, or <c>null</c> if no queued jobs are waiting.
    ///
    /// <para>Concurrent callers (multiple workers) serialize on the row lock; each call returns a
    /// different job. Implementation uses <c>SELECT ... FOR UPDATE</c> inside a transaction to
    /// avoid lost-update races.</para>
    /// </summary>
    public CrawlJob? TryClaimNextQueued()
    {
        // Connection-scoped transaction. With autocommit enabled (Dapper default), explicit
        // BeginTransaction is required to make the SELECT ... FOR UPDATE row-lock outlive the SELECT.
        // Dapper auto-opens for Query/Execute but BeginTransaction needs the connection open already.
        if (conn.State != ConnectionState.Open)
            conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            var job = conn.QueryFirstOrDefault<CrawlJob>(
                @"SELECT * FROM crawl_job
                   WHERE Status = 'Queued'
                   ORDER BY EnqueuedAt ASC
                   LIMIT 1
                   FOR UPDATE",
                transaction: tx);
            if (job == null)
            {
                tx.Commit();
                return null;
            }
            var startedAt = DateTime.UtcNow;
            conn.Execute(
                @"UPDATE crawl_job
                     SET Status = 'Running', StartedAt = @startedAt
                   WHERE Id = @id",
                new { id = job.Id, startedAt },
                transaction: tx);
            tx.Commit();
            job.Status = CrawlJobStatus.Running;
            job.StartedAt = startedAt;
            return job;
        }
        catch
        {
            try { tx.Rollback(); } catch { /* swallow rollback failure */ }
            throw;
        }
    }

    /// <summary>Transition to <see cref="CrawlJobStatus.Done"/>; sets <see cref="CrawlJob.CompletedAt"/>.</summary>
    public void MarkDone(string jobId)
    {
        if (string.IsNullOrWhiteSpace(jobId)) return;
        conn.Execute(
            "UPDATE crawl_job SET Status = 'Done', CompletedAt = @now, Error = NULL WHERE Id = @jobId",
            new { jobId, now = DateTime.UtcNow });
    }

    /// <summary>Transition to <see cref="CrawlJobStatus.Failed"/>; sets <see cref="CrawlJob.Error"/> and CompletedAt.</summary>
    public void MarkFailed(string jobId, string error)
    {
        if (string.IsNullOrWhiteSpace(jobId)) return;
        conn.Execute(
            "UPDATE crawl_job SET Status = 'Failed', CompletedAt = @now, Error = @error WHERE Id = @jobId",
            new { jobId, error = error ?? string.Empty, now = DateTime.UtcNow });
    }

    /// <summary>Transition to <see cref="CrawlJobStatus.Cancelled"/>; sets CompletedAt.</summary>
    public void MarkCancelled(string jobId)
    {
        if (string.IsNullOrWhiteSpace(jobId)) return;
        conn.Execute(
            "UPDATE crawl_job SET Status = 'Cancelled', CompletedAt = @now WHERE Id = @jobId",
            new { jobId, now = DateTime.UtcNow });
    }

    /// <summary>
    /// Returns the most-recent open (Queued or Running) job for a vendor, or null.
    /// Used by enqueue-time coalescing to drop a duplicate request and by status polling.
    /// </summary>
    public CrawlJob? GetOpenJobForVendor(string vendorId)
    {
        if (string.IsNullOrWhiteSpace(vendorId)) return null;
        return conn.QueryFirstOrDefault<CrawlJob>(
            @"SELECT * FROM crawl_job
               WHERE VendorId = @vendorId
                 AND Status IN ('Queued','Running')
               ORDER BY EnqueuedAt DESC
               LIMIT 1",
            new { vendorId });
    }

    /// <summary>Queue depth (rows still in <see cref="CrawlJobStatus.Queued"/>).</summary>
    public int CountQueued()
    {
        return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM crawl_job WHERE Status = 'Queued'");
    }

    /// <summary>Recent job history (most-recent first), bounded by <paramref name="limit"/>.</summary>
    public IEnumerable<CrawlJob> Recent(int limit = 50)
    {
        if (limit <= 0) limit = 50;
        return conn.Query<CrawlJob>(
            "SELECT * FROM crawl_job ORDER BY EnqueuedAt DESC LIMIT @limit",
            new { limit });
    }

    /// <summary>
    /// Reaps rows stuck in <see cref="CrawlJobStatus.Running"/> past <paramref name="staleThreshold"/>
    /// by transitioning them to <see cref="CrawlJobStatus.Failed"/> with a sentinel error. Returns the
    /// number of rows reaped.
    ///
    /// <para>Used by <c>CrawlWorkerService</c> at startup and periodically to clean up jobs orphaned
    /// when the host process dies (systemd SIGKILL on shutdown timeout, OOM, hard crash) — paths
    /// that bypass the normal catch/finally bookkeeping in <c>ExecuteJobAsync</c>. Safe to run
    /// concurrently with active workers: rows just-claimed have <c>StartedAt = UtcNow</c>, so they
    /// fall outside any sensible staleness threshold.</para>
    /// </summary>
    public int ReapStuckRunning(TimeSpan staleThreshold)
    {
        var now = DateTime.UtcNow;
        var cutoff = now - staleThreshold;
        return conn.Execute(
            @"UPDATE crawl_job
                 SET Status      = 'Failed',
                     CompletedAt = @now,
                     Error       = CONCAT('reaped_stuck_running',
                                          CASE WHEN Error IS NOT NULL AND Error <> ''
                                               THEN CONCAT(' | original: ', Error)
                                               ELSE '' END)
               WHERE Status    = 'Running'
                 AND StartedAt < @cutoff",
            new { now, cutoff });
    }
}
