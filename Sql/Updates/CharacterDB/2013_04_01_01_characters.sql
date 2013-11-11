ALTER TABLE `characters`
  ADD COLUMN `RealmId` int(11) NOT NULL DEFAULT '1' AFTER `AccountId`;
