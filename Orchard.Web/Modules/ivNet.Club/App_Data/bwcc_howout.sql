
CREATE TABLE bwcc_howout (
  id int NOT NULL,
  name nvarchar(20) NOT NULL default '',
  PRIMARY KEY  (id)
);
GO;

-- 
-- Dumping data for table 'bwcc_howout'
-- 

INSERT INTO bwcc_howout (id, name) VALUES (1, 'Bowled');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (2, 'Caught');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (3, 'LBW');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (4, 'Run Out');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (5, 'Stumped');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (6, 'C&B');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (7, 'Hit Wicket');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (8, 'Retired');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (9, 'Hit Ball Twice');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (10, 'Handled Ball');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (11, 'Obstructing Field');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (12, 'Timed Out');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (98, 'DNB');
GO;
INSERT INTO bwcc_howout (id, name) VALUES (99, 'Not Out');
GO;
