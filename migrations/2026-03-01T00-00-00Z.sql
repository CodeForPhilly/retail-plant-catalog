-- Migration: Add unique constraint on (VendorId, Uri) in vendor_urls to prevent duplicate plant listing URLs per vendor
-- Created: 2026-03-01
-- Run after deploying duplicate-prevention in app layer; remove any existing duplicates first.

-- Remove duplicate (VendorId, Uri) rows, keeping one row per pair (smallest Id)
DELETE v1 FROM vendor_urls v1
INNER JOIN vendor_urls v2
  ON v1.VendorId = v2.VendorId
  AND LOWER(TRIM(v1.Uri)) = LOWER(TRIM(v2.Uri))
  AND v1.Id > v2.Id;

-- Add unique constraint (MariaDB/MySQL: index is case-sensitive by default; app layer normalizes URIs)
ALTER TABLE vendor_urls
  ADD UNIQUE KEY `uk_vendor_urls_vendor_uri` (`VendorId`, `Uri`);
