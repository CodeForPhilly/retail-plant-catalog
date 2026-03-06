-- Migration: Add LivePlant and Seed booleans to vendor (Live Plant defaults to TRUE)
-- Created: 2026-03-01
-- Existing rows get LivePlant=1 and Seed=0 via DEFAULTs.

ALTER TABLE vendor
  ADD COLUMN LivePlant TINYINT(1) NOT NULL DEFAULT 1,
  ADD COLUMN Seed TINYINT(1) NOT NULL DEFAULT 0;
