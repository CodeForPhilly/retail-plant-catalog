-- MariaDB dump 10.19-11.2.2-MariaDB, for osx10.19 (arm64)
--
-- Host: localhost    Database: pac
-- ------------------------------------------------------
-- Server version	11.2.2-MariaDB-1:11.2.2+maria~ubu2204

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `_migrations`
--

DROP TABLE IF EXISTS `_migrations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `_migrations` (
  `key` varchar(20) NOT NULL,
  `hash` varchar(50) NOT NULL,
  PRIMARY KEY (`key`),
  UNIQUE KEY `hash_UNIQUE` (`hash`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `api_info`
--

DROP TABLE IF EXISTS `api_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `api_info` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(60) DEFAULT NULL,
  `UserId` char(38) NOT NULL,
  `OrganizationName` varchar(255) NOT NULL,
  `Phone` varchar(16) NOT NULL,
  `Address` varchar(1000) NOT NULL,
  `IntendedUse` text NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `Lng` decimal(10,8) NOT NULL,
  `Lat` decimal(10,8) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `idx_UserId` (`UserId`),
  CONSTRAINT `fk_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `plant`
--

DROP TABLE IF EXISTS `plant`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `plant` (
  `Id` char(38) NOT NULL,
  `Symbol` varchar(20) DEFAULT NULL,
  `RecommendationScore` decimal(10,0) NOT NULL,
  `Showy` bit(1) NOT NULL,
  `SuperPlant` bit(1) NOT NULL,
  `ScientificName` varchar(100) NOT NULL,
  `CommonName` varchar(100) NOT NULL,
  `FloweringMonths` varchar(20) DEFAULT NULL,
  `Height` varchar(15) DEFAULT NULL,
  `ImageUrl` varchar(500) DEFAULT NULL,
  `HasImage` bit(1) NOT NULL DEFAULT b'0',
  `HasPreview` bit(1) NOT NULL,
  `Source` varchar(30) DEFAULT NULL,
  `Attribution` text DEFAULT NULL,
  `Blurb` varchar(1000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `idx_Symbol` (`Symbol`),
  KEY `idx_ScientificName` (`ScientificName`),
  KEY `idx_CommonName` (`CommonName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `plant_state`
--

DROP TABLE IF EXISTS `plant_state`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `plant_state` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `State` char(2) NOT NULL,
  `PlantId` char(38) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_PlantId` (`PlantId`),
  CONSTRAINT `fk_PlantId` FOREIGN KEY (`PlantId`) REFERENCES `plant` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user` (
  `Id` char(38) NOT NULL,
  `Email` varchar(320) NOT NULL,
  `HashedPassword` varchar(150) DEFAULT NULL,
  `Role` enum('User','Admin','Vendor') DEFAULT 'User',
  `CreatedAt` datetime NOT NULL,
  `ModifiedAt` datetime NOT NULL,
  `ApiKey` char(38) DEFAULT NULL,
  `Verified` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `idx_Email` (`Email`),
  KEY `idx_ApiKey` (`ApiKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user_invite`
--

DROP TABLE IF EXISTS `user_invite`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user_invite` (
  `Id` char(38) NOT NULL,
  `UserId` char(38) NOT NULL,
  `ExpiresAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_user` (`UserId`),
  CONSTRAINT `fk_user` FOREIGN KEY (`UserId`) REFERENCES `user` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `vendor`
--

DROP TABLE IF EXISTS `vendor`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vendor` (
  `Id` char(38) NOT NULL,
  `UserId` char(38) DEFAULT NULL,
  `StoreName` varchar(255) DEFAULT NULL,
  `Geo` point NOT NULL,
  `Lat` decimal(10,8) DEFAULT NULL,
  `Lng` decimal(10,8) DEFAULT NULL,
  `Approved` tinyint(1) NOT NULL DEFAULT 0,
  `Address` varchar(500) DEFAULT NULL,
  `State` char(2) DEFAULT NULL,
  `StoreUrl` varchar(500) DEFAULT NULL,
  `PublicEmail` varchar(255) DEFAULT NULL,
  `PublicPhone` varchar(15) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `idx_StoreName` (`StoreName`),
  KEY `idx_State` (`State`,`Approved`),
  SPATIAL KEY `idx_geo` (`Geo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`%`*/ /*!50003 TRIGGER `update_geo_trigger` BEFORE INSERT ON `vendor` FOR EACH ROW BEGIN
    SET NEW.Geo = POINT(NEW.Lat, NEW.Lng);
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`%`*/ /*!50003 TRIGGER `update_geo_trigger_update` BEFORE UPDATE ON `vendor` FOR EACH ROW BEGIN
    SET NEW.Geo = POINT(NEW.Lat, NEW.Lng);
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `vendor_plant`
--

DROP TABLE IF EXISTS `vendor_plant`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vendor_plant` (
  `Id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `VendorId` char(38) NOT NULL,
  `PlantId` char(38) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uni_key` (`VendorId`,`PlantId`),
  KEY `fk_Vendor` (`VendorId`),
  KEY `fk_Plant` (`PlantId`),
  CONSTRAINT `fk_Plant` FOREIGN KEY (`PlantId`) REFERENCES `plant` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `fk_Vendor` FOREIGN KEY (`VendorId`) REFERENCES `vendor` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `vendor_urls`
--

DROP TABLE IF EXISTS `vendor_urls`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vendor_urls` (
  `Id` char(38) DEFAULT NULL,
  `Uri` varchar(500) DEFAULT NULL,
  `VendorId` char(38) DEFAULT NULL,
  KEY `fk_vendor_id` (`VendorId`),
  CONSTRAINT `fk_vendor_url` FOREIGN KEY (`VendorId`) REFERENCES `vendor` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `zip`
--

DROP TABLE IF EXISTS `zip`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `zip` (
  `Code` char(5) NOT NULL,
  `Lat` double NOT NULL,
  `Lng` double NOT NULL,
  `City` varchar(255) DEFAULT NULL,
  `State` char(2) NOT NULL,
  `StateFull` varchar(255) DEFAULT NULL,
  `Population` int(11) DEFAULT NULL,
  `CountyFips` int(11) DEFAULT NULL,
  `CountyName` varchar(255) DEFAULT NULL,
  `Geo` point DEFAULT NULL,
  PRIMARY KEY (`Code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`%`*/ /*!50003 TRIGGER `zip_before_insert` BEFORE INSERT ON `zip` FOR EACH ROW BEGIN
    SET NEW.Geo = POINT(NEW.Lat, NEW.Lng);
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`%`*/ /*!50003 TRIGGER `zip_before_update` BEFORE UPDATE ON `zip` FOR EACH ROW BEGIN
    SET NEW.Geo = POINT(NEW.Lat, NEW.Lng);
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-01-28 11:08:03
