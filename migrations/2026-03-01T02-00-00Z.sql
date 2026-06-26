-- Migration: Add index on vendor_urls.LastSucceeded for scheduled uncrawled-URL queries
-- Created: 2026-03-01

CREATE INDEX idx_vendor_urls_last_succeeded ON vendor_urls (LastSucceeded);
