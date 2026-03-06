-- Migration: Add CrawlInProgress to vendor and vendor_urls for UI inactive state and concurrent-crawl prevention
-- Created: 2026-03-01

ALTER TABLE vendor
  ADD COLUMN CrawlInProgress TINYINT(1) NOT NULL DEFAULT 0;

ALTER TABLE vendor_urls
  ADD COLUMN CrawlInProgress TINYINT(1) NOT NULL DEFAULT 0;
