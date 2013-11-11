ALTER TABLE `gameobject_spawns`
    CHANGE COLUMN `activated` `state` tinyint(4) NOT NULL DEFAULT '0';
