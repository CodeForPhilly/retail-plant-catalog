-- 2026-06-18: Partner attribute on vendor (CO 010 Item 1, Option A).
--
-- Adds:
--   * `partner` lookup table — enum-by-row, avoids ENUM ALTER pain when partners
--     are added later. Seeded with 'Xerces' only.
--   * vendor.Partner       (varchar, FK -> partner.Id)
--   * vendor.ExternalKey   (varchar, the partner-side ID; for Xerces this is a
--     deterministic hash of normalized name+state since the Xerces CSV has no
--     stable native ID — derived in the sync skill, not here)
--   * UNIQUE (Partner, ExternalKey) so re-running the Xerces sync is idempotent.
--     MariaDB treats multi-NULL unique tuples as distinct, so vendors with no
--     partner (Partner=NULL) do NOT collide with each other.
--
-- Existing rows keep Partner=NULL, ExternalKey=NULL. The Vendor Insert/Update
-- repository methods are intentionally NOT changed in this migration's
-- companion C# release: partner linkage is set via a dedicated SetPartnerLink
-- method so the regular vendor save flow cannot accidentally clear it.
--
-- Idempotent: information_schema guards on every ALTER, ON DUPLICATE KEY UPDATE
-- on the seed insert, CREATE TABLE IF NOT EXISTS on the lookup. Safe to re-run.

-- ---------------------------------------------------------------------------
-- 1. partner lookup table
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `partner` (
  `Id`          varchar(50)  NOT NULL,
  `DisplayName` varchar(100) NOT NULL,
  `Active`      tinyint(1)   NOT NULL DEFAULT 1,
  `CreatedAt`   datetime     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_general_ci;

INSERT INTO `partner` (`Id`, `DisplayName`, `Active`, `CreatedAt`)
VALUES ('Xerces', 'Xerces Society Native Plant Directory', 1, UTC_TIMESTAMP())
ON DUPLICATE KEY UPDATE
  `DisplayName` = VALUES(`DisplayName`),
  `Active`      = VALUES(`Active`);

-- ---------------------------------------------------------------------------
-- 2. vendor.Partner column (idempotent via information_schema)
-- ---------------------------------------------------------------------------
SET @col_partner = (
  SELECT COUNT(*) FROM information_schema.COLUMNS
   WHERE TABLE_SCHEMA = DATABASE()
     AND TABLE_NAME   = 'vendor'
     AND COLUMN_NAME  = 'Partner'
);
SET @sql = IF(@col_partner = 0,
              'ALTER TABLE `vendor` ADD COLUMN `Partner` varchar(50) DEFAULT NULL',
              'SELECT "vendor.Partner already present"');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- 3. vendor.ExternalKey column
-- ---------------------------------------------------------------------------
SET @col_extkey = (
  SELECT COUNT(*) FROM information_schema.COLUMNS
   WHERE TABLE_SCHEMA = DATABASE()
     AND TABLE_NAME   = 'vendor'
     AND COLUMN_NAME  = 'ExternalKey'
);
SET @sql = IF(@col_extkey = 0,
              'ALTER TABLE `vendor` ADD COLUMN `ExternalKey` varchar(255) DEFAULT NULL',
              'SELECT "vendor.ExternalKey already present"');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- 4. Foreign key vendor.Partner -> partner.Id
-- ---------------------------------------------------------------------------
SET @fk_exists = (
  SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
   WHERE TABLE_SCHEMA   = DATABASE()
     AND TABLE_NAME     = 'vendor'
     AND CONSTRAINT_NAME= 'fk_vendor_partner'
);
SET @sql = IF(@fk_exists = 0,
              'ALTER TABLE `vendor` ADD CONSTRAINT `fk_vendor_partner` FOREIGN KEY (`Partner`) REFERENCES `partner`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE',
              'SELECT "fk_vendor_partner already present"');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ---------------------------------------------------------------------------
-- 5. UNIQUE INDEX (Partner, ExternalKey) — enforces idempotent partner imports
-- ---------------------------------------------------------------------------
SET @idx_exists = (
  SELECT COUNT(*) FROM information_schema.STATISTICS
   WHERE TABLE_SCHEMA = DATABASE()
     AND TABLE_NAME   = 'vendor'
     AND INDEX_NAME   = 'ux_vendor_partner_external_key'
);
SET @sql = IF(@idx_exists = 0,
              'CREATE UNIQUE INDEX `ux_vendor_partner_external_key` ON `vendor` (`Partner`, `ExternalKey`)',
              'SELECT "ux_vendor_partner_external_key already present"');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
