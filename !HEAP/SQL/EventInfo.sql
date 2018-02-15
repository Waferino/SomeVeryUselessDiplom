CREATE TABLE `www0005_base`.`eventinfo` (
  `idEventInfo` INT NOT NULL AUTO_INCREMENT,
  `DateOfThe` DATETIME NULL,
  `Name` VARCHAR(150) NOT NULL,
  `Notation` LONGTEXT NULL,
  PRIMARY KEY (`idEventInfo`))
COMMENT = 'Dictionary for Events names and dates';