IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'DeviceManagement')
BEGIN
    CREATE DATABASE DeviceManagement;
END
GO

USE DeviceManagement;
GO


IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
CREATE TABLE dbo.Users (
                           Id       INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
                           Name     NVARCHAR(100) NOT NULL,
                           Role     NVARCHAR(50)  NOT NULL,
                           Location NVARCHAR(100) NOT NULL
);
END
GO


IF OBJECT_ID('dbo.Devices', 'U') IS NULL
BEGIN
CREATE TABLE dbo.Devices (
                             Id              INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
                             Name            NVARCHAR(100) NOT NULL,
                             Manufacturer    NVARCHAR(100) NOT NULL,
                             Type            INT           NOT NULL,   
                             OperatingSystem NVARCHAR(50)  NOT NULL,
                             OSVersion       NVARCHAR(50)  NOT NULL,
                             Processor       NVARCHAR(100) NOT NULL,
                             RAM             INT           NOT NULL, 
                             Description     NVARCHAR(500) NULL
);
END
GO


IF OBJECT_ID('dbo.DeviceAssignments', 'U') IS NULL
BEGIN
CREATE TABLE dbo.DeviceAssignments (
                                       Id           INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
                                       DeviceId     INT           NOT NULL,
                                       UserId       INT           NOT NULL,
                                       Location     NVARCHAR(100) NOT NULL,
                                       AssignedDate DATETIME2     NOT NULL,
                                       ReturnedDate DATETIME2     NULL,

                                       CONSTRAINT FK_DeviceAssignments_Devices
                                           FOREIGN KEY (DeviceId) REFERENCES dbo.Devices(Id) ON DELETE CASCADE,

                                       CONSTRAINT FK_DeviceAssignments_Users
                                           FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
);
END
GO
