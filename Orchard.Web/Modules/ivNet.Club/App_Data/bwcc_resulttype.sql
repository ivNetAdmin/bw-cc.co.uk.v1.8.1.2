
CREATE TABLE bwcc_resulttype (
  id int NOT NULL,
  name nvarchar(20) NOT NULL default '',
  PRIMARY KEY  (id)
);
GO;

-- 
-- Dumping data for table 'bwcc_resulttype'
-- 

INSERT INTO bwcc_resulttype (id, name) VALUES (1, 'Win');
GO;
INSERT INTO bwcc_resulttype (id, name) VALUES (2, 'Lose');
GO;
INSERT INTO bwcc_resulttype (id, name) VALUES (3, 'Draw');
GO;
INSERT INTO bwcc_resulttype (id, name) VALUES (4, 'Conceded');
GO;
INSERT INTO bwcc_resulttype (id, name) VALUES (5, 'Walk Over');
GO;
INSERT INTO bwcc_resulttype (id, name) VALUES (10, 'Abandoned');
GO;
INSERT INTO bwcc_resulttype (id, name) VALUES (11, 'Cancelled');
GO;
