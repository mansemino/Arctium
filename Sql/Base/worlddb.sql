/*
Navicat MySQL Data Transfer

Source Server         : local
Source Server Version : 50519
Source Host           : localhost:3306
Source Database       : world

Target Server Type    : MYSQL
Target Server Version : 50519
File Encoding         : 65001

Date: 2013-05-07 04:22:12
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `broadcast_texts`
-- ----------------------------
DROP TABLE IF EXISTS `broadcast_texts`;
CREATE TABLE `broadcast_texts` (
  `Id` int(11) NOT NULL,
  `Language` int(11) NOT NULL,
  `Text` text,
  `AlternativeText` text,
  `Emote0` int(11) NOT NULL,
  `Emote1` int(11) NOT NULL,
  `Emote2` int(11) NOT NULL,
  `Emote3` int(11) NOT NULL,
  `Emote4` int(11) NOT NULL,
  `Emote5` int(11) NOT NULL,
  `Emote6` int(11) NOT NULL,
  `Emote7` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of broadcast_texts
-- ----------------------------
INSERT INTO `broadcast_texts` VALUES ('56417', '0', 'Shouldn\'t you be training, young $c?$B$BDiscipline your mind and body, and there is little that you cannot achieve.', null, '0', '0', '0', '0', '0', '0', '0', '0');

-- ----------------------------
-- Table structure for `creature_data`
-- ----------------------------
DROP TABLE IF EXISTS `creature_data`;
CREATE TABLE `creature_data` (
  `Id` int(11) NOT NULL,
  `Health` int(11) NOT NULL DEFAULT '1',
  `Level` tinyint(4) NOT NULL DEFAULT '1',
  `Class` tinyint(4) NOT NULL DEFAULT '1',
  `Faction` int(11) NOT NULL DEFAULT '35',
  `Scale` float NOT NULL DEFAULT '1',
  `UnitFlags` int(11) NOT NULL DEFAULT '0',
  `UnitFlags2` int(11) NOT NULL DEFAULT '0',
  `NpcFlags` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of creature_data
-- ----------------------------
INSERT INTO `creature_data` VALUES ('57754', '408', '5', '1', '35', '1', '33536', '2048', '1');

-- ----------------------------
-- Table structure for `creature_gossips`
-- ----------------------------
DROP TABLE IF EXISTS `creature_gossips`;
CREATE TABLE `creature_gossips` (
  `Guid` bigint(20) NOT NULL,
  `GossipDataId` int(11) NOT NULL,
  `BroadcastTextId` int(11) NOT NULL,
  PRIMARY KEY (`Guid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of creature_gossips
-- ----------------------------
INSERT INTO `creature_gossips` VALUES ('1', '13346', '56417');

-- ----------------------------
-- Table structure for `creature_spawns`
-- ----------------------------
DROP TABLE IF EXISTS `creature_spawns`;
CREATE TABLE `creature_spawns` (
  `guid` bigint(20) NOT NULL AUTO_INCREMENT,
  `id` int(10) NOT NULL,
  `map` int(11) NOT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `o` float NOT NULL,
  PRIMARY KEY (`guid`),
  KEY `creatureId` (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of creature_spawns
-- ----------------------------
INSERT INTO `creature_spawns` VALUES ('1', '57754', '860', '1449.7', '3406.6', '171.092', '2.2868');

-- ----------------------------
-- Table structure for `creature_stats`
-- ----------------------------
DROP TABLE IF EXISTS `creature_stats`;
CREATE TABLE `creature_stats` (
  `Id` int(10) NOT NULL,
  `Name` text,
  `SubName` text,
  `IconName` text,
  `Flag` int(11) NOT NULL DEFAULT '0',
  `Flag2` int(11) NOT NULL DEFAULT '0',
  `Type` int(10) NOT NULL DEFAULT '0',
  `Family` int(11) NOT NULL DEFAULT '0',
  `Rank` int(11) NOT NULL DEFAULT '0',
  `QuestKillNpcId` int(11) NOT NULL DEFAULT '0',
  `QuestKillNpcId2` int(11) NOT NULL DEFAULT '0',
  `DisplayInfoId` int(11) NOT NULL,
  `DisplayInfoId2` int(11) NOT NULL DEFAULT '0',
  `DisplayInfoId3` int(11) NOT NULL DEFAULT '0',
  `DisplayInfoId4` int(11) NOT NULL DEFAULT '0',
  `HealthModifier` float NOT NULL DEFAULT '1',
  `PowerModifier` float NOT NULL DEFAULT '1',
  `RacialLeader` tinyint(4) NOT NULL DEFAULT '0',
  `QuestItemId` int(11) NOT NULL DEFAULT '0',
  `QuestItemId2` int(11) NOT NULL DEFAULT '0',
  `QuestItemId3` int(11) NOT NULL DEFAULT '0',
  `QuestItemId4` int(11) NOT NULL DEFAULT '0',
  `QuestItemId5` int(11) NOT NULL DEFAULT '0',
  `QuestItemId6` int(11) NOT NULL DEFAULT '0',
  `MovementInfoId` int(11) NOT NULL DEFAULT '0',
  `ExpansionRequired` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Records of creature_stats
-- ----------------------------
INSERT INTO `creature_stats` VALUES ('57754', 'Instructor Mossthorn', '', '', '0', '0', '7', '0', '0', '0', '0', '39795', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0');

-- ----------------------------
-- Table structure for `gameobject_gossips`
-- ----------------------------
DROP TABLE IF EXISTS `gameobject_gossips`;
CREATE TABLE `gameobject_gossips` (
  `Guid` bigint(20) NOT NULL,
  `GossipDataId` int(11) NOT NULL,
  `BroadcastTextId` int(11) NOT NULL,
  PRIMARY KEY (`Guid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of gameobject_gossips
-- ----------------------------

-- ----------------------------
-- Table structure for `gameobject_spawns`
-- ----------------------------
DROP TABLE IF EXISTS `gameobject_spawns`;
CREATE TABLE `gameobject_spawns` (
  `guid` bigint(20) NOT NULL AUTO_INCREMENT,
  `id` int(10) NOT NULL,
  `map` int(11) NOT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `o` float NOT NULL,
  `factionTemplate` int(10) unsigned NOT NULL,
  `animProgress` smallint(6) NOT NULL,
  `state` tinyint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`guid`),
  KEY `creatureId` (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of gameobject_spawns
-- ----------------------------

-- ----------------------------
-- Table structure for `gameobject_stats`
-- ----------------------------
DROP TABLE IF EXISTS `gameobject_stats`;
CREATE TABLE `gameobject_stats` (
  `Id` int(10) NOT NULL,
  `Type` int(11) NOT NULL DEFAULT '0',
  `Flags` int(11) NOT NULL,
  `DisplayInfoId` int(11) NOT NULL DEFAULT '0',
  `Name` text,
  `IconName` text,
  `CastBarCaption` text,
  `Data` int(11) NOT NULL DEFAULT '0',
  `Data2` int(11) NOT NULL DEFAULT '0',
  `Data3` int(11) NOT NULL DEFAULT '0',
  `Data4` int(11) NOT NULL DEFAULT '0',
  `Data5` int(11) NOT NULL,
  `Data6` int(11) NOT NULL DEFAULT '0',
  `Data7` int(11) NOT NULL DEFAULT '0',
  `Data8` int(11) NOT NULL DEFAULT '0',
  `Data9` int(11) NOT NULL,
  `Data10` int(11) NOT NULL,
  `Data11` int(11) NOT NULL,
  `Data12` int(11) NOT NULL,
  `Data13` int(11) NOT NULL,
  `Data14` int(11) NOT NULL,
  `Data15` int(11) NOT NULL,
  `Data16` int(11) NOT NULL,
  `Data17` int(11) NOT NULL,
  `Data18` int(11) NOT NULL,
  `Data19` int(11) NOT NULL,
  `Data20` int(11) NOT NULL,
  `Data21` int(11) NOT NULL,
  `Data22` int(11) NOT NULL,
  `Data23` int(11) NOT NULL,
  `Data24` int(11) NOT NULL,
  `Data25` int(11) NOT NULL,
  `Data26` int(11) NOT NULL,
  `Data27` int(11) NOT NULL,
  `Data28` int(11) NOT NULL,
  `Data29` int(11) NOT NULL,
  `Data30` int(11) NOT NULL,
  `Data31` int(11) NOT NULL,
  `Data32` int(11) NOT NULL,
  `Size` float NOT NULL DEFAULT '0',
  `QuestItemId` int(11) NOT NULL DEFAULT '0',
  `QuestItemId2` int(11) NOT NULL DEFAULT '0',
  `QuestItemId3` int(11) NOT NULL DEFAULT '0',
  `QuestItemId4` int(11) NOT NULL DEFAULT '0',
  `QuestItemId5` int(11) NOT NULL DEFAULT '0',
  `QuestItemId6` int(11) NOT NULL DEFAULT '0',
  `ExpansionRequired` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=COMPACT;

-- ----------------------------
-- Records of gameobject_stats
-- ----------------------------

-- ----------------------------
-- Table structure for `gossip_data`
-- ----------------------------
DROP TABLE IF EXISTS `gossip_data`;
CREATE TABLE `gossip_data` (
  `Id` int(11) NOT NULL,
  `FriendshipFactionID` int(11) NOT NULL,
  `TextID` int(11) NOT NULL,
  `OptionsCount` int(11) NOT NULL,
  `QuestsCount` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of gossip_data
-- ----------------------------
INSERT INTO `gossip_data` VALUES ('13346', '0', '18919', '0', '0');

-- ----------------------------
-- Table structure for `teleport_locations`
-- ----------------------------
DROP TABLE IF EXISTS `teleport_locations`;
CREATE TABLE `teleport_locations` (
  `location` varchar(255) NOT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `o` float NOT NULL,
  `map` int(10) unsigned NOT NULL,
  PRIMARY KEY (`location`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of teleport_locations
-- ----------------------------
