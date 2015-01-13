
CREATE TABLE bwcc_contacttypes (
  id int NOT NULL,
  name nvarchar(50) NOT NULL default '',
  PRIMARY KEY  (id)
);
GO;

-- 
-- Dumping data for table 'bwcc_contacttypes'
-- 

INSERT INTO bwcc_contacttypes (id, name) VALUES (1, 'Mother');
GO;
INSERT INTO bwcc_contacttypes (id, name) VALUES (2, 'Father');
GO;
INSERT INTO bwcc_contacttypes (id, name) VALUES (3, 'Doctor');
GO;
INSERT INTO bwcc_contacttypes (id, name) VALUES (4, 'Dentist');
GO;
INSERT INTO bwcc_contacttypes (id, name) VALUES (5, 'Unknown');
GO;
INSERT INTO bwcc_contacttypes (id, name) VALUES (6, 'Alternative');
GO;
