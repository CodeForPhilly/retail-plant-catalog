-- User registration invites (admin-initiated, 72h TTL, distinct from user_invite verification tokens)
CREATE TABLE IF NOT EXISTS `registration_invite` (
  `Id` char(38) NOT NULL,
  `Token` varchar(64) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `ExpiresAt` datetime NOT NULL,
  `Status` varchar(20) NOT NULL DEFAULT 'Pending',
  `InvitedByUserId` char(38) DEFAULT NULL,
  `AcceptedUserId` char(38) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_registration_invite_token` (`Token`),
  KEY `idx_registration_invite_email_status` (`Email`, `Status`),
  KEY `fk_registration_invite_inviter` (`InvitedByUserId`),
  CONSTRAINT `fk_registration_invite_inviter` FOREIGN KEY (`InvitedByUserId`) REFERENCES `user` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
