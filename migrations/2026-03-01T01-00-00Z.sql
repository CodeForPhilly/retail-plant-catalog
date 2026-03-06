-- Migration: Add PlantCount to vendor_urls to track plants found per URL
-- Created: 2026-03-01

ALTER TABLE vendor_urls
  ADD COLUMN PlantCount INT NULL DEFAULT NULL AFTER LastStatus;
