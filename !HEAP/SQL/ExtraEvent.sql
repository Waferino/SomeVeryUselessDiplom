CREATE TABLE `www0005_base`.`extraevent` (
  `id_ExtraEvent` INT NOT NULL AUTO_INCREMENT,
  `id_Event` INT NOT NULL,
  `fileName` VARCHAR(80) NOT NULL,
  `contentType` VARCHAR(45) NOT NULL,
  `fileDataPath` VARCHAR(160) NOT NULL,
  `creatingDate` VARCHAR(23) NOT NULL,
  PRIMARY KEY (`id_ExtraEvent`),
  UNIQUE INDEX `fileDataPath_UNIQUE` (`fileDataPath` ASC),
  INDEX `FK_id_Event_idx` (`id_Event` ASC),
  CONSTRAINT `FK_id_Event`
    FOREIGN KEY (`id_Event`)
    REFERENCES `www0005_base`.`event` (`id_Event`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
COMMENT = 'Extras for Student\'s Events';
