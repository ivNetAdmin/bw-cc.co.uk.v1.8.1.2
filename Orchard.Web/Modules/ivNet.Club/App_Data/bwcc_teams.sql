
CREATE TABLE bwcc_teams (
  id int NOT NULL,
  name nvarchar(50) NOT NULL default '',
  captainid int NOT NULL default '0',
  PRIMARY KEY  (id)
);
GO;

-- 
-- Dumping data for table 'bwcc_teams'
-- 

INSERT INTO bwcc_teams (id, name, captainid) VALUES (1, 'Sat 1st XI', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (2, 'Sun 1st XI', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (3, 'Sat 2nd XI', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (4, 'U17', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (5, 'U16', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (6, 'U15', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (7, 'U14', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (8, 'U13A', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (9, 'U13B', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (10, 'U12A', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (11, 'U12B', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (12, 'U11A', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (13, 'U11B', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (14, 'U11C', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (15, 'U10A', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (16, 'U10B', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (17, 'U10C', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (18, 'U9', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (19, 'U8', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (20, 'U7', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (21, 'U6', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (22, 'Waggonnettes', 0);
GO;
INSERT INTO bwcc_teams (id, name, captainid) VALUES (23, 'Waggonnettes B', 0);
GO;
