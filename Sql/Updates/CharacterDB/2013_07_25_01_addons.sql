ALTER TABLE `addons`
	DROP COLUMN `Index`,
	DROP PRIMARY KEY,
	ADD PRIMARY KEY (`Name`);