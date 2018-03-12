CREATE TABLE `www0005_base`.`eventinfo` (
  `id_EventInfo` INT NOT NULL AUTO_INCREMENT,
  `DateOfThe` DATETIME NULL,
  `Name` VARCHAR(150) NOT NULL,
  `Notation` LONGTEXT NULL,
  `ChangedBy` INT NOT NULL,
  PRIMARY KEY (`idEventInfo`))
COMMENT = 'Dictionary for Events names and dates';