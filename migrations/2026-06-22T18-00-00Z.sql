-- 2026-06-22: partner_audit table.
--
-- Records every tag / update / clear operation on a vendor's Partner+ExternalKey
-- linkage. Append-only: no UPDATE or DELETE paths exposed by the application.
-- Read by Admin only via a future audit-read endpoint (not part of this migration).
--
-- The Operation column is denormalised from the prior/new value pair for
-- ergonomic queries; the values are always consistent with the prior/new pair.

CREATE TABLE IF NOT EXISTS `partner_audit` (
  `Id`                char(38)     NOT NULL,
  `VendorId`          char(38)     NOT NULL,
  `Operation`         enum('Tag','Update','Clear') NOT NULL,
  `PriorPartner`      varchar(50)  DEFAULT NULL,
  `PriorExternalKey`  varchar(255) DEFAULT NULL,
  `NewPartner`        varchar(50)  DEFAULT NULL,
  `NewExternalKey`    varchar(255) DEFAULT NULL,
  `ActingUserId`      char(38)     NOT NULL,
  `OccurredAt`        datetime     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `idx_partner_audit_vendor`   (`VendorId`),
  KEY `idx_partner_audit_occurred` (`OccurredAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
