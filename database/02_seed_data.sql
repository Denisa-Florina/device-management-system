USE DeviceManagement;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users)
BEGIN
    INSERT INTO dbo.Users (Name, Role, Location) VALUES
        (N'Alice Johnson', N'Developer',       N'New York'),
        (N'Bob Smith',     N'QA Engineer',     N'London'),
        (N'Carol White',   N'Project Manager', N'Berlin'),
        (N'David Brown',   N'Designer',        N'Paris'),
        (N'Eva Martinez',  N'DevOps Engineer', N'Madrid');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Devices)
BEGIN
    INSERT INTO dbo.Devices (Name, Manufacturer, Type, OperatingSystem, OSVersion, Processor, RAM, Description) VALUES
        (N'iPhone 15 Pro',    N'Apple',     0, N'iOS',     N'17.4', N'Apple A17 Pro',       8,  N'128GB storage'),
        (N'Galaxy S24 Ultra', N'Samsung',   0, N'Android', N'14',   N'Snapdragon 8 Gen 3', 12, N'256GB storage'),
        (N'iPad Pro 12.9',    N'Apple',     1, N'iPadOS',  N'17.4', N'Apple M2',           16, N'512GB storage'),
        (N'Pixel 8 Pro',      N'Google',    0, N'Android', N'14',   N'Google Tensor G3',   12, N'256GB storage'),
        (N'Galaxy Tab S9',    N'Samsung',   1, N'Android', N'14',   N'Snapdragon 8 Gen 2', 12, N'256GB storage'),
        (N'OnePlus 12',       N'OnePlus',   0, N'Android', N'14',   N'Snapdragon 8 Gen 3', 12, N'256GB storage'),
        (N'iPhone 14',        N'Apple',     0, N'iOS',     N'17.4', N'Apple A15 Bionic',    6, N'128GB storage'),
        (N'Surface Pro 9',    N'Microsoft', 1, N'Windows', N'11',   N'Intel Core i7-1255U',16, N'512GB SSD'),
        (N'Xperia 1 V',       N'Sony',      0, N'Android', N'14',   N'Snapdragon 8 Gen 2', 12, N'256GB storage'),
        (N'Galaxy A54',       N'Samsung',   0, N'Android', N'14',   N'Exynos 1380',         8, N'128GB storage');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DeviceAssignments)
BEGIN
    INSERT INTO dbo.DeviceAssignments (DeviceId, UserId, Location, AssignedDate, ReturnedDate) VALUES
        (1, 1, N'New York', '2024-01-10 09:00:00', NULL),
        (2, 2, N'London',   '2024-02-15 10:30:00', NULL),
        (3, 3, N'Berlin',   '2024-03-01 08:00:00', NULL),
        (5, 4, N'Paris',    '2024-03-20 14:00:00', NULL);
END
GO
