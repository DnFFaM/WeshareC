--CREATE DATABASE WeShareC; 

--Use WeShareC;

--CREATE TABLE UserInfo(
--	Id INT IDENTITY(1,1) PRIMARY KEY,
--	UserName NVARCHAR(25),
--	Pass NVARCHAR(25)
--);

--INSERT INTO UserInfo (UserName, Pass) VALUES ('AAd', '11lol1');
--INSERT INTO UserInfo (UserName, Pass) VALUES ('LAa', '2124s');
--INSERT INTO UserInfo (UserName, Pass) VALUES ('OaA', '3rws3');

--CREATE TABLE Purchases(
--	Id INT IDENTITY(1,1) PRIMARY KEY,
--	UserName NVARCHAR(25),
--	Item NVARCHAR(25),
--	Price INT
--);


--CREATE TABLE GroupData (
--    GroupID INT PRIMARY KEY IDENTITY,
--    GroupName VARCHAR(50) NOT NULL
--);

--CREATE TABLE Groups (
--    GroupID INT PRIMARY KEY IDENTITY,
--    GroupName VARCHAR(50) NOT NULL
--);

--CREATE TRIGGER trg_InsertGroupData
--ON GroupData
--AFTER INSERT
--AS
--BEGIN
--    SET NOCOUNT ON;

--    INSERT INTO Groups (GroupName)
--    SELECT GroupName
--    FROM inserted;
--END;

--ALTER TABLE GroupData
--ADD UserName NVARCHAR(25),


--ALTER TABLE Purchases
--ADD GroupName NVARCHAR(25);

--ALTER TABLE Purchases
--DROP COLUMN Price;

--ALTER TABLE Purchases
--Add Price numeric(6, 2);


--TRUNCATE TABLE GroupData;
--TRUNCATE TABLE Groups;
--TRUNCATE TABLE Purchases;
--TRUNCATE TABLE UserInfo;


--ALTER TABLE GroupData
--ADD UserId NVARCHAR(25);

--UPDATE GroupData
--SET GroupData.UserId = userinfo.id
--FROM GroupData
--JOIN userinfo ON GroupData.UserName = userinfo.username;


--ALTER TABLE Groups
--ADD UserId NVARCHAR(25);

--UPDATE Groups
--SET UserId = userinfo.id
--FROM GroupData
--JOIN userinfo ON GroupData.UserId = userinfo.Id;


--CREATE TRIGGER trg_UpdateUserId
--ON GroupData
--AFTER INSERT, UPDATE
--AS
--BEGIN
--    UPDATE gd
--    SET gd.UserId = ui.id
--    FROM GroupData gd
--    JOIN userinfo ui ON gd.UserName = ui.username
--    JOIN inserted ins ON gd.GroupID = ins.GroupID;
--END;


--CREATE TRIGGER trg_UpdateUserIdGroups
--ON Groups
--AFTER INSERT
--AS
--BEGIN
--    UPDATE g
--    SET g.UserId = ui.UserId
--    FROM Groups g
--    JOIN GroupData ui ON g.UserId = ui.UserId
--    JOIN inserted ins ON g.GroupID = ins.GroupID;
--END;

--DROP TRIGGER trg_UpdateUserIdGroups;


COLLATE SQL_Latin1_General_CP1_CI_AS