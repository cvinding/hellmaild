CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `Mails` (
    `id` int NOT NULL AUTO_INCREMENT,
    `subject` longtext NULL,
    `message` longtext NULL,
    CONSTRAINT `PK_Mails` PRIMARY KEY (`id`)
);

CREATE TABLE `Users` (
    `id` int NOT NULL AUTO_INCREMENT,
    `firstname` longtext NULL,
    `surname` longtext NULL,
    `email` longtext NULL,
    `password` longtext NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`id`)
);

CREATE TABLE `Mails_Users` (
    `id` int NOT NULL AUTO_INCREMENT,
    `mail_id` int NULL,
    `from_user_id` int NULL,
    `to_user_id` int NULL,
    `recipient_type` int NOT NULL DEFAULT 0,
    CONSTRAINT `PK_Mails_Users` PRIMARY KEY (`id`),
    CONSTRAINT `FK_Mails_Users_Users_from_user_id` FOREIGN KEY (`from_user_id`) REFERENCES `Users` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Mails_Users_Mails_mail_id` FOREIGN KEY (`mail_id`) REFERENCES `Mails` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Mails_Users_Users_to_user_id` FOREIGN KEY (`to_user_id`) REFERENCES `Users` (`id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_Mails_Users_from_user_id` ON `Mails_Users` (`from_user_id`);

CREATE INDEX `IX_Mails_Users_mail_id` ON `Mails_Users` (`mail_id`);

CREATE INDEX `IX_Mails_Users_to_user_id` ON `Mails_Users` (`to_user_id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20190425104446_init', '2.2.3-servicing-35854');

