ALTER TABLE `character_creation_data`
  ADD COLUMN `baseMap` smallint(6) NOT NULL DEFAULT '0' AFTER `class`;

UPDATE `character_creation_data` SET `map` = 638, `baseMap` = 654 WHERE `race` = '22' AND `class` <> '6';
