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
        (N'iPhone 15 Pro',       N'Apple',     0, N'iOS',      N'17.4', N'Apple A17 Pro',        8,  N'128GB storage'),
        (N'Galaxy S24 Ultra',    N'Samsung',   0, N'Android',  N'14',   N'Snapdragon 8 Gen 3',  12, N'256GB storage'),
        (N'iPad Pro 12.9',       N'Apple',     1, N'iPadOS',   N'17.4', N'Apple M2',            16, N'512GB storage'),
        (N'Pixel 8 Pro',         N'Google',    0, N'Android',  N'14',   N'Google Tensor G3',    12, N'256GB storage'),
        (N'Galaxy Tab S9',       N'Samsung',   1, N'Android',  N'14',   N'Snapdragon 8 Gen 2',  12, N'256GB storage'),
        (N'OnePlus 12',          N'OnePlus',   0, N'Android',  N'14',   N'Snapdragon 8 Gen 3',  12, N'256GB storage'),
        (N'iPhone 14',           N'Apple',     0, N'iOS',      N'17.4', N'Apple A15 Bionic',     6, N'128GB storage'),
        (N'Surface Pro 9',       N'Microsoft', 1, N'Windows',  N'11',   N'Intel Core i7-1255U', 16, N'512GB SSD'),
        (N'Xperia 1 V',         N'Sony',      0, N'Android',  N'14',   N'Snapdragon 8 Gen 2',  12, N'256GB storage'),
        (N'Galaxy A54',          N'Samsung',   0, N'Android',  N'14',   N'Exynos 1380',          8, N'128GB storage'),
        (N'iPhone 16 Pro Max',   N'Apple',     0, N'iOS',      N'18.0', N'Apple A18 Pro',        8, N'256GB storage'),
        (N'Pixel 9',             N'Google',    0, N'Android',  N'15',   N'Google Tensor G4',    12, N'128GB storage'),
        (N'Galaxy Z Fold 5',     N'Samsung',   0, N'Android',  N'14',   N'Snapdragon 8 Gen 2',  12, N'256GB, foldable'),
        (N'Galaxy Z Flip 5',     N'Samsung',   0, N'Android',  N'14',   N'Snapdragon 8 Gen 2',   8, N'256GB, foldable'),
        (N'iPad Air M2',         N'Apple',     1, N'iPadOS',   N'17.4', N'Apple M2',             8, N'256GB storage'),
        (N'iPad Mini 6',         N'Apple',     1, N'iPadOS',   N'17.4', N'Apple A15 Bionic',     4, N'64GB storage'),
        (N'Xiaomi 14 Ultra',     N'Xiaomi',    0, N'Android',  N'14',   N'Snapdragon 8 Gen 3',  16, N'512GB, Leica camera'),
        (N'Nothing Phone 2',     N'Nothing',   0, N'Android',  N'14',   N'Snapdragon 8+ Gen 1', 12, N'256GB storage'),
        (N'Motorola Edge 50 Pro',N'Motorola',  0, N'Android',  N'14',   N'Snapdragon 7 Gen 3',  12, N'256GB storage'),
        (N'Huawei MatePad Pro',  N'Huawei',    1, N'HarmonyOS',N'4.0',  N'Kirin 9000S',         12, N'256GB storage'),
        (N'Lenovo Tab P12 Pro',  N'Lenovo',    1, N'Android',  N'13',   N'Snapdragon 870',       8, N'256GB, OLED display'),
        (N'Nokia G60',           N'Nokia',     0, N'Android',  N'13',   N'Snapdragon 695',       6, N'128GB storage'),
        (N'Asus ROG Phone 8',    N'Asus',      0, N'Android',  N'14',   N'Snapdragon 8 Gen 3',  16, N'512GB, gaming phone'),
        (N'Oppo Find X7 Ultra',  N'Oppo',      0, N'Android',  N'14',   N'Dimensity 9300',      16, N'256GB, Hasselblad camera'),
        (N'Realme GT 5 Pro',     N'Realme',    0, N'Android',  N'14',   N'Snapdragon 8 Gen 3',  12, N'256GB storage');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.DeviceAssignments)
BEGIN
    INSERT INTO dbo.DeviceAssignments (DeviceId, UserId, Location, AssignedDate, ReturnedDate) VALUES
        (1,  1, N'New York', '2024-01-10 09:00:00', NULL),
        (2,  2, N'London',   '2024-02-15 10:30:00', NULL),
        (3,  3, N'Berlin',   '2024-03-01 08:00:00', NULL),
        (5,  4, N'Paris',    '2024-03-20 14:00:00', NULL),
        (11, 5, N'Madrid',   '2024-04-01 09:00:00', NULL),
        (17, 1, N'New York', '2024-04-10 11:00:00', NULL),
        (23, 2, N'London',   '2024-05-01 08:30:00', NULL);
END
GO
