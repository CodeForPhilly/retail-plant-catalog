/* up */
ALTER TABLE user MODIFY COLUMN Role ENUM('User', 'Admin', 'Vendor', 'Volunteer', 'VolunteerPlus') NOT NULL DEFAULT 'User';
UPDATE user SET Role = 'Volunteer' WHERE Role = 'Vendor';
ALTER TABLE user MODIFY COLUMN Role ENUM('User', 'Admin', 'Volunteer', 'VolunteerPlus') NOT NULL DEFAULT 'User';
