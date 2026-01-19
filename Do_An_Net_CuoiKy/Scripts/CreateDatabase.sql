-- =============================================
-- Script tạo Database cho Shop Thú Cưng MVC
-- Database: ShopThuCungDB
-- =============================================

-- Tạo Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ShopThuCungDB')
BEGIN
    CREATE DATABASE ShopThuCungDB;
END
GO

USE ShopThuCungDB;
GO

-- =============================================
-- Bảng Users (Người dùng)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Email] NVARCHAR(100) NOT NULL UNIQUE,
        [Password] NVARCHAR(255) NOT NULL,
        [FullName] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(20) NULL,
        [Address] NVARCHAR(255) NULL,
        [Role] NVARCHAR(50) NOT NULL DEFAULT 'Customer',
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
    );
    
    CREATE INDEX IX_Users_Email ON [Users](Email);
END
GO

-- =============================================
-- Bảng Categories (Danh mục)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Categories] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [ImageUrl] NVARCHAR(255) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    );
END
GO

-- =============================================
-- Bảng Products (Sản phẩm)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Products] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Name] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(2000) NULL,
        [Price] DECIMAL(18,2) NOT NULL,
        [OldPrice] DECIMAL(18,2) NULL,
        [ImageUrl] NVARCHAR(255) NULL,
        [CategoryId] INT NOT NULL,
        [Stock] INT NOT NULL DEFAULT 0,
        [Rating] DECIMAL(3,2) NOT NULL DEFAULT 0,
        [ReviewCount] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([Id])
    );
    
    CREATE INDEX IX_Products_CategoryId ON [Products](CategoryId);
END
GO

-- =============================================
-- Bảng Orders (Đơn hàng)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Orders] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [OrderDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [TotalAmount] DECIMAL(18,2) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [ShippingAddress] NVARCHAR(500) NULL,
        [ShippingName] NVARCHAR(100) NULL,
        [ShippingPhone] NVARCHAR(20) NULL,
        [Notes] NVARCHAR(500) NULL,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
    
    CREATE INDEX IX_Orders_UserId ON [Orders](UserId);
END
GO

-- =============================================
-- Bảng OrderDetails (Chi tiết đơn hàng)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OrderDetails] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [OrderId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [Quantity] INT NOT NULL,
        [Price] DECIMAL(18,2) NOT NULL,
        FOREIGN KEY ([OrderId]) REFERENCES [Orders]([Id]) ON DELETE CASCADE,
        FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id])
    );
    
    CREATE INDEX IX_OrderDetails_OrderId ON [OrderDetails](OrderId);
    CREATE INDEX IX_OrderDetails_ProductId ON [OrderDetails](ProductId);
END
GO

-- =============================================
-- Bảng Cart (Giỏ hàng)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Carts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Carts] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [ProductId] INT NOT NULL,
        [Quantity] INT NOT NULL DEFAULT 1,
        [AddedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
        FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]) ON DELETE CASCADE,
        UNIQUE ([UserId], [ProductId])
    );
    
    CREATE INDEX IX_Carts_UserId ON [Carts](UserId);
    CREATE INDEX IX_Carts_ProductId ON [Carts](ProductId);
END
GO

-- =============================================
-- Bảng Reviews (Đánh giá)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reviews]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Reviews] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [ProductId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [Rating] INT NOT NULL CHECK ([Rating] >= 1 AND [Rating] <= 5),
        [Comment] NVARCHAR(1000) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id]) ON DELETE CASCADE,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
        UNIQUE ([UserId], [ProductId])
    );
    
    CREATE INDEX IX_Reviews_ProductId ON [Reviews](ProductId);
    CREATE INDEX IX_Reviews_UserId ON [Reviews](UserId);
END
GO

-- =============================================
-- INSERT DỮ LIỆU MẪU
-- =============================================

-- Insert Categories
IF NOT EXISTS (SELECT * FROM Categories WHERE Id = 1)
BEGIN
    INSERT INTO Categories (Id, Name, Description, IsActive) VALUES
    (N'Thức Ăn', N'Thức ăn dinh dưỡng cho thú cưng', 1),
    (N'Đồ Chơi', N'Đồ chơi vui nhộn cho thú cưng', 1),
    (N'Phụ Kiện', N'Phụ kiện thời trang cho thú cưng', 1),
    (N'Chăm Sóc', N'Sản phẩm chăm sóc sức khỏe', 1);
END
GO

-- Insert Products
IF NOT EXISTS (SELECT * FROM Products WHERE Id = 1)
BEGIN
    INSERT INTO Products (Id, Name, Description, Price, OldPrice, CategoryId, Stock, Rating, ReviewCount, IsActive) VALUES
    (1, N'Thức Ăn Khô Cho Chó', N'Thức ăn khô chất lượng cao, đầy đủ dinh dưỡng cho chó', 250000, 300000, 1, 100, 4.5, 120, 1),
    (2, N'Đồ Chơi Xương Gặm', N'Xương gặm giúp làm sạch răng và giải trí cho chó', 150000, NULL, 2, 50, 4.0, 80, 1),
    (3, N'Vòng Cổ Thời Trang', N'Vòng cổ đẹp mắt, chất liệu cao cấp', 180000, NULL, 3, 75, 5.0, 45, 1),
    (4, N'Sữa Tắm Cho Chó', N'Sữa tắm dịu nhẹ, làm sạch và thơm mát', 120000, NULL, 4, 60, 4.5, 95, 1);
END
GO

-- Insert Admin User (Password: Admin123! - đã hash)
-- Lưu ý: Trong thực tế cần hash password bằng BCrypt hoặc Identity
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@shopthucung.com')
BEGIN
    INSERT INTO Users (Email, Password, FullName, Role, CreatedDate) VALUES
    (N'admin@shopthucung.com', N'$2a$11$YourHashedPasswordHere', N'Administrator', N'Admin', GETDATE());
END
GO

PRINT 'Database ShopThuCungDB đã được tạo thành công!';
PRINT 'Các bảng đã được tạo và dữ liệu mẫu đã được insert.';
GO
