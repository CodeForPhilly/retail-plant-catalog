-- Migration: Add UrlParsingError to LastStatus ENUM in vendor_urls table
-- Created: 2026-02-03

ALTER TABLE vendor_urls MODIFY COLUMN LastStatus ENUM('Timeout','DnsFailure','Redirect','Missing','RobotDenied','None','Ok','UrlParsingError') NULL;
