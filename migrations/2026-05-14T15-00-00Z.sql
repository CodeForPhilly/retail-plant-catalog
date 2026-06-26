-- Migration: add crawl_job table for queue-backed crawl execution.
-- Generated 2026-05-14 for the crawl-queue feature branch.
--
-- Schema is additive only.

CREATE TABLE IF NOT EXISTS `crawl_job` (
  `Id`            char(38) NOT NULL,
  `VendorId`      char(38) NOT NULL,
  `UrlId`         char(38) DEFAULT NULL,
  `Status`        enum('Queued','Running','Done','Failed','Cancelled')
                  NOT NULL DEFAULT 'Queued',
  `Source`        enum('UI','MCP','Schedule','Bulk','Other')
                  NOT NULL DEFAULT 'Other',
  `RequestedBy`   char(38) DEFAULT NULL,
  `EnqueuedAt`    datetime NOT NULL,
  `StartedAt`     datetime DEFAULT NULL,
  `CompletedAt`   datetime DEFAULT NULL,
  `Error`         text     DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `idx_crawl_job_status_enqueued` (`Status`, `EnqueuedAt`),
  KEY `idx_crawl_job_vendor` (`VendorId`),
  KEY `idx_crawl_job_url` (`UrlId`)
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_general_ci;
