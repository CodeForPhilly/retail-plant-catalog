-- 2026-06-17: one-time backfill for the CrawlWorkerService stuck-Running leak.
--
-- The queue-backed scheduled crawl correctly calls MarkDone/MarkFailed in its
-- try/catch/finally, but process death (systemd SIGKILL on shutdown timeout,
-- OOM, crash) bypasses all of it: the OS tears down the process before the
-- catch runs, and prior to this release the new process did not reap
-- pre-existing Running rows on startup.
--
-- Symptoms in production:
--   * Persistent spinner icons in the Vendors list.
--   * Silent degradation of nightly scheduled-crawl coverage — vendors with
--     CrawlInProgress=1 are skipped by ScheduledCrawlService, so the per-night
--     enqueue count trends downward as the leak accumulates.
--
-- The shipping CrawlWorkerService release adds a startup reap + periodic janitor
-- (default: reap Running > 60min, sweep every 15min). This migration is a one-
-- time cleanup of the historical backlog so prod returns to a clean baseline
-- immediately rather than waiting an hour for the janitor.
--
-- Safe to re-run. Idempotent: the Status='Running' filter means already-Failed
-- rows are not re-touched, and the NOT EXISTS subquery on the vendor sweep only
-- clears flags that no longer have any open job row.

-- Step 1: mark stuck Running rows as Failed. Preserve the original Error (if any)
-- behind the sentinel marker so a later audit can distinguish backfilled rows.
UPDATE crawl_job
   SET Status      = 'Failed',
       CompletedAt = UTC_TIMESTAMP(),
       Error       = CONCAT(
         'reaped_stuck_running_phase5_leak_backfill',
         CASE WHEN Error IS NOT NULL AND Error <> ''
              THEN CONCAT(' | original: ', Error)
              ELSE '' END
       )
 WHERE Status    = 'Running'
   AND StartedAt < UTC_TIMESTAMP() - INTERVAL 1 HOUR;

-- Step 2: clear vendor.CrawlInProgress for vendors with no remaining open jobs.
-- Must run AFTER step 1 so any rows reaped above no longer count as "open."
UPDATE vendor v
   SET v.CrawlInProgress = FALSE
 WHERE v.CrawlInProgress = TRUE
   AND NOT EXISTS (
     SELECT 1 FROM crawl_job cj
      WHERE cj.VendorId = v.Id
        AND cj.Status IN ('Queued','Running')
   );
