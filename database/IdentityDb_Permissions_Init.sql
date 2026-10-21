-- ============================================================================
-- SCRIPT TẠO BẢNG & SEED DANH MỤC PHÂN QUYỀN (PERMISSIONS) CHO IDENTITYDB
-- Project: eProject
-- Compatible with: SQL Server 2019 / 2022 / Azure SQL
-- ============================================================================

USE [IdentityDb];
GO

-- ============================================================================
-- 1. BẢNG DANH MỤC QUYỀN HẠN (Permissions)
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Permissions] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Code] NVARCHAR(100) NOT NULL,              -- Mã quyền duy nhất (vd: Users.Create, Orders.Approve)
        [Name] NVARCHAR(200) NOT NULL,              -- Tên hiển thị tiếng Việt (vd: Tạo tài khoản mới)
        [GroupName] NVARCHAR(100) NOT NULL,         -- Nhóm chức năng hiển thị trên UI (vd: Quản lý Người dùng)
        [Description] NVARCHAR(500) NULL,           -- Mô tả chi tiết chức năng
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Permissions_Code] 
    ON [dbo].[Permissions] ([Code] ASC);

    CREATE NONCLUSTERED INDEX [IX_Permissions_GroupName] 
    ON [dbo].[Permissions] ([GroupName] ASC);

    PRINT N'Table [Permissions] created successfully.';
END
ELSE
BEGIN
    PRINT N'Table [Permissions] already exists.';
END
GO

-- ============================================================================
-- 2. BẢNG GÁN QUYỀN CHO VAI TRÒ (RolePermissions)
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolePermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RolePermissions] (
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [PermissionId] UNIQUEIDENTIFIER NOT NULL,
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED ([RoleId] ASC, [PermissionId] ASC),
        CONSTRAINT [FK_RolePermissions_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) 
            REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) 
            REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_RolePermissions_PermissionId] 
    ON [dbo].[RolePermissions] ([PermissionId] ASC);

    PRINT N'Table [RolePermissions] created successfully.';
END
ELSE
BEGIN
    PRINT N'Table [RolePermissions] already exists.';
END
GO

-- ============================================================================
-- 3. BẢNG GÁN/CHẶN QUYỀN RIÊNG THEO TỪNG TÀI KHOẢN (UserPermissions - Mở rộng)
-- Cho phép gán quyền ngoại lệ hoặc thu hồi riêng cho 1 User mà không cần đổi Role
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserPermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserPermissions] (
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [PermissionId] UNIQUEIDENTIFIER NOT NULL,
        [IsGranted] BIT NOT NULL DEFAULT 1,          -- 1: Cấp quyền riêng lẻ, 0: Thu hồi/Chặn quyền (Deny)
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT [PK_UserPermissions] PRIMARY KEY CLUSTERED ([UserId] ASC, [PermissionId] ASC),
        CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) 
            REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_UserPermissions_PermissionId] 
    ON [dbo].[UserPermissions] ([PermissionId] ASC);

    PRINT N'Table [UserPermissions] created successfully.';
END
ELSE
BEGIN
    PRINT N'Table [UserPermissions] already exists.';
END
GO

-- ============================================================================
-- 4. SEED DỮ LIỆU DANH MỤC QUYỀN HẠN (Permissions)
-- Sử dụng MERGE để có thể chạy nhiều lần mà không bị trùng lặp dữ liệu
-- ============================================================================
PRINT N'Seeding default permissions...';

DECLARE @PermissionsData TABLE (
    [Code] NVARCHAR(100),
    [Name] NVARCHAR(200),
    [GroupName] NVARCHAR(100),
    [Description] NVARCHAR(500)
);

INSERT INTO @PermissionsData ([Code], [Name], [GroupName], [Description]) VALUES
-- Nhóm: Quản lý Người dùng
('Users.View', N'Xem danh sách tài khoản', N'Quản lý Người dùng', N'Cho phép xem danh sách và thông tin chi tiết người dùng'),
('Users.Create', N'Tạo tài khoản mới', N'Quản lý Người dùng', N'Cho phép thêm mới người dùng vào hệ thống'),
('Users.Edit', N'Chỉnh sửa tài khoản', N'Quản lý Người dùng', N'Cho phép cập nhật thông tin cá nhân và trạng thái tài khoản'),
('Users.Delete', N'Xóa / Khóa tài khoản', N'Quản lý Người dùng', N'Cho phép xóa hoặc khóa/mở khóa tài khoản người dùng'),
('Users.AssignRole', N'Gán vai trò tài khoản', N'Quản lý Người dùng', N'Cho phép thay đổi vai trò (Role) của người dùng'),

-- Nhóm: Quản lý Phân quyền
('Roles.View', N'Xem danh sách vai trò', N'Quản lý Phân quyền', N'Cho phép xem danh sách vai trò và quyền hạn hiện có'),
('Roles.Create', N'Tạo vai trò mới', N'Quản lý Phân quyền', N'Cho phép tạo vai trò chức năng mới trong hệ thống'),
('Roles.Edit', N'Cập nhật vai trò & quyền', N'Quản lý Phân quyền', N'Cho phép đổi tên và cấu hình ma trận quyền cho vai trò'),
('Roles.Delete', N'Xóa vai trò', N'Quản lý Phân quyền', N'Cho phép xóa vai trò (chỉ khi không có user nào đang dùng)'),

-- Nhóm: Hàng hóa & Danh mục
('Catalog.Products.View', N'Xem sản phẩm', N'Hàng hóa & Danh mục', N'Cho phép xem danh sách và chi tiết sản phẩm'),
('Catalog.Products.Create', N'Thêm mới sản phẩm', N'Hàng hóa & Danh mục', N'Cho phép thêm mới sản phẩm vào hệ thống'),
('Catalog.Products.Edit', N'Chỉnh sửa sản phẩm', N'Hàng hóa & Danh mục', N'Cho phép chỉnh sửa thông tin, giá, tồn kho sản phẩm'),
('Catalog.Products.Delete', N'Xóa sản phẩm', N'Hàng hóa & Danh mục', N'Cho phép xóa hoặc ẩn sản phẩm khỏi hệ thống'),
('Catalog.Categories.Manage', N'Quản lý danh mục hàng hóa', N'Hàng hóa & Danh mục', N'Cho phép tạo, sửa, xóa danh mục sản phẩm'),

-- Nhóm: Quản lý Đơn hàng
('Orders.View', N'Xem danh sách đơn hàng', N'Quản lý Đơn hàng', N'Cho phép xem thông tin và lịch sử đơn hàng'),
('Orders.Create', N'Tạo đơn hàng mới', N'Quản lý Đơn hàng', N'Cho phép lập đơn hàng mới'),
('Orders.Edit', N'Chỉnh sửa đơn hàng', N'Quản lý Đơn hàng', N'Cho phép cập nhật trạng thái và chi tiết đơn hàng'),
('Orders.Cancel', N'Hủy đơn hàng', N'Quản lý Đơn hàng', N'Cho phép hủy đơn hàng theo quy định'),
('Orders.Approve', N'Duyệt đơn hàng', N'Quản lý Đơn hàng', N'Cho phép phê duyệt và xác nhận xử lý đơn hàng'),

-- Nhóm: Báo cáo & Thống kê
('Reports.View', N'Xem báo cáo & thống kê', N'Báo cáo & Thống kê', N'Cho phép xem các biểu đồ thống kê doanh số, số lượng'),
('Reports.Export', N'Xuất báo cáo', N'Báo cáo & Thống kê', N'Cho phép xuất dữ liệu báo cáo ra file Excel hoặc PDF'),

-- Nhóm: Quản trị Hệ thống
('System.Settings.View', N'Xem cấu hình hệ thống', N'Quản trị Hệ thống', N'Cho phép xem các tham số và cấu hình hệ thống'),
('System.Settings.Manage', N'Cập nhật cấu hình hệ thống', N'Quản trị Hệ thống', N'Cho phép chỉnh sửa các thiết lập và tham số vận hành'),
('System.AuditLog.View', N'Xem nhật ký hoạt động', N'Quản trị Hệ thống', N'Cho phép tra cứu lịch sử thao tác của các tài khoản');

MERGE [dbo].[Permissions] AS Target
USING @PermissionsData AS Source
ON (Target.[Code] = Source.[Code])
WHEN MATCHED THEN
    UPDATE SET 
        Target.[Name] = Source.[Name],
        Target.[GroupName] = Source.[GroupName],
        Target.[Description] = Source.[Description]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Id], [Code], [Name], [GroupName], [Description])
    VALUES (NEWID(), Source.[Code], Source.[Name], Source.[GroupName], Source.[Description]);

PRINT N'Permissions seeded successfully.';
GO

-- ============================================================================
-- 5. SEED GÁN QUYỀN MẶC ĐỊNH CHO CÁC VAI TRÒ (ADMIN, MANAGER, USER)
-- ============================================================================
PRINT N'Seeding Role-Permissions mappings...';

DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'ADMIN');
DECLARE @ManagerRoleId UNIQUEIDENTIFIER = (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'MANAGER');
DECLARE @UserRoleId UNIQUEIDENTIFIER = (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'USER');

-- 5.1 Gán TẤT CẢ quyền cho Role ADMIN
IF @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId])
    SELECT @AdminRoleId, p.[Id]
    FROM [dbo].[Permissions] p
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[RolePermissions] rp 
        WHERE rp.[RoleId] = @AdminRoleId AND rp.[PermissionId] = p.[Id]
    );
    PRINT N'All permissions granted to [Admin].';
END

-- 5.2 Gán quyền quản lý nghiệp vụ cho Role MANAGER
IF @ManagerRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId])
    SELECT @ManagerRoleId, p.[Id]
    FROM [dbo].[Permissions] p
    WHERE p.[Code] IN (
        'Users.View',
        'Catalog.Products.View', 'Catalog.Products.Create', 'Catalog.Products.Edit', 'Catalog.Categories.Manage',
        'Orders.View', 'Orders.Create', 'Orders.Edit', 'Orders.Approve',
        'Reports.View', 'Reports.Export',
        'System.Settings.View'
    )
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[RolePermissions] rp 
        WHERE rp.[RoleId] = @ManagerRoleId AND rp.[PermissionId] = p.[Id]
    );
    PRINT N'Business permissions granted to [Manager].';
END

-- 5.3 Gán quyền cơ bản cho Role USER
IF @UserRoleId IS NOT NULL
BEGIN
    INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId])
    SELECT @UserRoleId, p.[Id]
    FROM [dbo].[Permissions] p
    WHERE p.[Code] IN (
        'Catalog.Products.View',
        'Orders.View',
        'Orders.Create'
    )
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[RolePermissions] rp 
        WHERE rp.[RoleId] = @UserRoleId AND rp.[PermissionId] = p.[Id]
    );
    PRINT N'Basic permissions granted to [User].';
END
GO

PRINT N'============================================================================';
PRINT N'SUCCESS: Permissions tables and seed data created successfully!';
PRINT N'============================================================================';
GO
