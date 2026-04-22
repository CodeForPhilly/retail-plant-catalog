-- registration_invite.Role (idempotent for DBs created before Role existed)
ALTER TABLE `registration_invite` ADD COLUMN `Role` varchar(32) NOT NULL DEFAULT 'User' AFTER `Email`