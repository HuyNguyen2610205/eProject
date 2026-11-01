-- ============================================================================
-- DATABASE MIGRATION SCRIPT FOR GIVE-AID NGO PLATFORM
-- Database: IdentityDb (Microsoft SQL Server 2019/2022)
-- Description: Cập nhật cấu trúc database đáp ứng yêu cầu SRS Give-AID NGO Website
-- ============================================================================

USE [IdentityDb];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

BEGIN TRANSACTION;
BEGIN TRY

    PRINT N'>>> BẮT ĐẦU MIGRATION DATABASE CHO GIVE-AID NGO WEBSITE...';

    -- ========================================================================
    -- 1. CẬP NHẬT BẢNG AspNetUsers: THÊM CỘT Occupation
    -- ========================================================================
    IF NOT EXISTS (
        SELECT 1 FROM sys.columns 
        WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND name = 'Occupation'
    )
    BEGIN
        ALTER TABLE [dbo].[AspNetUsers] 
        ADD [Occupation] NVARCHAR(200) NULL;
        
        PRINT N'[OK] Đã thêm cột [Occupation] vào bảng [AspNetUsers].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Cột [Occupation] đã tồn tại trong [AspNetUsers].';
    END

    -- ========================================================================
    -- 2. BẢNG Causes: DANH MỤC QUYÊN GÓP
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Causes]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[Causes] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [Name] NVARCHAR(150) NOT NULL,
            [Slug] NVARCHAR(150) NOT NULL,
            [Description] NVARCHAR(1000) NULL,
            [IconUrl] NVARCHAR(500) NULL,
            [ImageUrl] NVARCHAR(500) NULL,
            [IsActive] BIT NOT NULL DEFAULT 1,
            [DisplayOrder] INT NOT NULL DEFAULT 0,
            [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            [UpdatedAt] DATETIME2(7) NULL,
            CONSTRAINT [PK_Causes] PRIMARY KEY CLUSTERED ([Id] ASC)
        );

        CREATE UNIQUE NONCLUSTERED INDEX [IX_Causes_Slug] ON [dbo].[Causes] ([Slug] ASC);
        CREATE NONCLUSTERED INDEX [IX_Causes_IsActive_DisplayOrder] ON [dbo].[Causes] ([IsActive] ASC, [DisplayOrder] ASC);

        PRINT N'[OK] Đã tạo bảng [Causes].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [Causes] đã tồn tại.';
    END

    -- ========================================================================
    -- 3. BẢNG Ngos: TỔ CHỨC PHI CHÍNH PHỦ
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ngos]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[Ngos] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [Name] NVARCHAR(250) NOT NULL,
            [Description] NVARCHAR(MAX) NULL,
            [LogoUrl] NVARCHAR(500) NULL,
            [Website] NVARCHAR(250) NULL,
            [ContactEmail] NVARCHAR(256) NULL,
            [Phone] NVARCHAR(50) NULL,
            [Address] NVARCHAR(500) NULL,
            [IsActive] BIT NOT NULL DEFAULT 1,
            [DisplayOrder] INT NOT NULL DEFAULT 0,
            [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            [UpdatedAt] DATETIME2(7) NULL,
            CONSTRAINT [PK_Ngos] PRIMARY KEY CLUSTERED ([Id] ASC)
        );

        CREATE NONCLUSTERED INDEX [IX_Ngos_IsActive_DisplayOrder] ON [dbo].[Ngos] ([IsActive] ASC, [DisplayOrder] ASC);
        PRINT N'[OK] Đã tạo bảng [Ngos].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [Ngos] đã tồn tại.';
    END

    -- ========================================================================
    -- 4. BẢNG NgoCauses: QUAN HỆ NHIỀU - NHIỀU GIỮA NGO VÀ DANH MỤC
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NgoCauses]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[NgoCauses] (
            [NgoId] INT NOT NULL,
            [CauseId] INT NOT NULL,
            [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            CONSTRAINT [PK_NgoCauses] PRIMARY KEY CLUSTERED ([NgoId] ASC, [CauseId] ASC),
            CONSTRAINT [FK_NgoCauses_Ngos_NgoId] FOREIGN KEY ([NgoId]) REFERENCES [dbo].[Ngos] ([Id]) ON DELETE CASCADE,
            CONSTRAINT [FK_NgoCauses_Causes_CauseId] FOREIGN KEY ([CauseId]) REFERENCES [dbo].[Causes] ([Id]) ON DELETE CASCADE
        );

        CREATE NONCLUSTERED INDEX [IX_NgoCauses_CauseId] ON [dbo].[NgoCauses] ([CauseId] ASC);
        PRINT N'[OK] Đã tạo bảng [NgoCauses].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [NgoCauses] đã tồn tại.';
    END

    -- ========================================================================
    -- 5. BẢNG Programmes: CÁC CHƯƠNG TRÌNH, HOẠT ĐỘNG
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Programmes]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[Programmes] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [NgoId] INT NOT NULL,
            [Title] NVARCHAR(250) NOT NULL,
            [Description] NVARCHAR(MAX) NULL,
            [Category] NVARCHAR(100) NULL,
            [Location] NVARCHAR(250) NULL,
            [StartDate] DATE NOT NULL,
            [EndDate] DATE NOT NULL,
            [ImageUrl] NVARCHAR(500) NULL,
            [Status] NVARCHAR(50) NOT NULL DEFAULT N'Upcoming',
            [IsFeatured] BIT NOT NULL DEFAULT 0,
            [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            [UpdatedAt] DATETIME2(7) NULL,
            CONSTRAINT [PK_Programmes] PRIMARY KEY CLUSTERED ([Id] ASC),
            CONSTRAINT [FK_Programmes_Ngos_NgoId] FOREIGN KEY ([NgoId]) REFERENCES [dbo].[Ngos] ([Id]) ON DELETE CASCADE,
            CONSTRAINT [CK_Programmes_Dates] CHECK ([EndDate] >= [StartDate]),
            CONSTRAINT [CK_Programmes_Status] CHECK ([Status] IN (N'Upcoming', N'Ongoing', N'Completed', N'Cancelled'))
        );

        CREATE NONCLUSTERED INDEX [IX_Programmes_NgoId] ON [dbo].[Programmes] ([NgoId] ASC);
        CREATE NONCLUSTERED INDEX [IX_Programmes_Status_StartDate] ON [dbo].[Programmes] ([Status] ASC, [StartDate] ASC);
        PRINT N'[OK] Đã tạo bảng [Programmes].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [Programmes] đã tồn tại.';
    END

    -- ========================================================================
    -- 6. BẢNG ProgrammeRegistrations: ĐĂNG KÝ THAM GIA CHƯƠNG TRÌNH
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProgrammeRegistrations]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[ProgrammeRegistrations] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [ProgrammeId] INT NOT NULL,
            [UserId] UNIQUEIDENTIFIER NULL,
            [FullName] NVARCHAR(150) NOT NULL,
            [Email] NVARCHAR(256) NOT NULL,
            [Phone] NVARCHAR(50) NULL,
            [Motivation] NVARCHAR(1000) NULL,
            [Status] NVARCHAR(50) NOT NULL DEFAULT N'Pending',
            [RegisteredAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            CONSTRAINT [PK_ProgrammeRegistrations] PRIMARY KEY CLUSTERED ([Id] ASC),
            CONSTRAINT [FK_ProgrammeRegistrations_Programmes_ProgrammeId] FOREIGN KEY ([ProgrammeId]) REFERENCES [dbo].[Programmes] ([Id]) ON DELETE CASCADE,
            CONSTRAINT [FK_ProgrammeRegistrations_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE SET NULL,
            CONSTRAINT [CK_ProgrammeRegistrations_Status] CHECK ([Status] IN (N'Pending', N'Approved', N'Rejected', N'Attended'))
        );

        CREATE UNIQUE NONCLUSTERED INDEX [IX_ProgrammeRegistrations_UserUnique] 
        ON [dbo].[ProgrammeRegistrations] ([ProgrammeId] ASC, [UserId] ASC) 
        WHERE [UserId] IS NOT NULL;

        CREATE NONCLUSTERED INDEX [IX_ProgrammeRegistrations_Programme_Email] 
        ON [dbo].[ProgrammeRegistrations] ([ProgrammeId] ASC, [Email] ASC);

        PRINT N'[OK] Đã tạo bảng [ProgrammeRegistrations].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [ProgrammeRegistrations] đã tồn tại.';
    END

    -- ========================================================================
    -- 7. BẢNG Donations: GIAO DỊCH QUYÊN GÓP (MÔ PHỎNG)
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Donations]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[Donations] (
            [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
            [TransactionCode] NVARCHAR(50) NOT NULL,
            [CauseId] INT NOT NULL,
            [UserId] UNIQUEIDENTIFIER NULL,
            [DonorName] NVARCHAR(150) NOT NULL,
            [DonorEmail] NVARCHAR(256) NOT NULL,
            [DonorPhone] NVARCHAR(50) NULL,
            [Amount] DECIMAL(18,2) NOT NULL,
            [PaymentMethod] NVARCHAR(50) NOT NULL DEFAULT N'MockCard',
            [CardLastFour] NVARCHAR(10) NULL,
            [Status] NVARCHAR(50) NOT NULL DEFAULT N'Pending',
            [Notes] NVARCHAR(500) NULL,
            [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            [CompletedAt] DATETIME2(7) NULL,
            CONSTRAINT [PK_Donations] PRIMARY KEY CLUSTERED ([Id] ASC),
            CONSTRAINT [FK_Donations_Causes_CauseId] FOREIGN KEY ([CauseId]) REFERENCES [dbo].[Causes] ([Id]),
            CONSTRAINT [FK_Donations_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE SET NULL,
            CONSTRAINT [CK_Donations_Amount] CHECK ([Amount] >= 10000.00 AND [Amount] <= 100000000.00),
            CONSTRAINT [CK_Donations_Status] CHECK ([Status] IN (N'Pending', N'Successful', N'Failed', N'Cancelled'))
        );

        CREATE UNIQUE NONCLUSTERED INDEX [IX_Donations_TransactionCode] ON [dbo].[Donations] ([TransactionCode] ASC);
        CREATE NONCLUSTERED INDEX [IX_Donations_UserId_CreatedAt] ON [dbo].[Donations] ([UserId] ASC, [CreatedAt] DESC);
        CREATE NONCLUSTERED INDEX [IX_Donations_CauseId_Status] ON [dbo].[Donations] ([CauseId] ASC, [Status] ASC);
        CREATE NONCLUSTERED INDEX [IX_Donations_Status_CreatedAt] ON [dbo].[Donations] ([Status] ASC, [CreatedAt] DESC);

        PRINT N'[OK] Đã tạo bảng [Donations].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [Donations] đã tồn tại.';
    END

    -- ========================================================================
    -- 8. BẢNG Partners: ĐỐI TÁC VÀ NHÀ TÀI TRỢ
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Partners]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[Partners] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [Name] NVARCHAR(200) NOT NULL,
            [LogoUrl] NVARCHAR(500) NOT NULL,
            [Website] NVARCHAR(250) NULL,
            [Description] NVARCHAR(500) NULL,
            [DisplayOrder] INT NOT NULL DEFAULT 0,
            [IsActive] BIT NOT NULL DEFAULT 1,
            [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            CONSTRAINT [PK_Partners] PRIMARY KEY CLUSTERED ([Id] ASC)
        );

        CREATE NONCLUSTERED INDEX [IX_Partners_IsActive_DisplayOrder] ON [dbo].[Partners] ([IsActive] ASC, [DisplayOrder] ASC);
        PRINT N'[OK] Đã tạo bảng [Partners].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [Partners] đã tồn tại.';
    END

    -- ========================================================================
    -- 9. BẢNG AboutPages: NỘI DUNG 7 TRANG ABOUT US
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AboutPages]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[AboutPages] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [Slug] NVARCHAR(100) NOT NULL,
            [Title] NVARCHAR(200) NOT NULL,
            [Content] NVARCHAR(MAX) NOT NULL,
            [BannerUrl] NVARCHAR(500) NULL,
            [IsPublished] BIT NOT NULL DEFAULT 1,
            [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            [UpdatedBy] UNIQUEIDENTIFIER NULL,
            CONSTRAINT [PK_AboutPages] PRIMARY KEY CLUSTERED ([Id] ASC),
            CONSTRAINT [FK_AboutPages_AspNetUsers_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE SET NULL
        );

        CREATE UNIQUE NONCLUSTERED INDEX [IX_AboutPages_Slug] ON [dbo].[AboutPages] ([Slug] ASC);
        PRINT N'[OK] Đã tạo bảng [AboutPages].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [AboutPages] đã tồn tại.';
    END

    -- ========================================================================
    -- 10. BẢNG Galleries: THƯ VIỆN HÌNH ẢNH
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Galleries]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[Galleries] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [ProgrammeId] INT NULL,
            [Title] NVARCHAR(200) NOT NULL,
            [ImageUrl] NVARCHAR(500) NOT NULL,
            [Caption] NVARCHAR(500) NULL,
            [DisplayOrder] INT NOT NULL DEFAULT 0,
            [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            CONSTRAINT [PK_Galleries] PRIMARY KEY CLUSTERED ([Id] ASC),
            CONSTRAINT [FK_Galleries_Programmes_ProgrammeId] FOREIGN KEY ([ProgrammeId]) REFERENCES [dbo].[Programmes] ([Id]) ON DELETE SET NULL
        );

        CREATE NONCLUSTERED INDEX [IX_Galleries_ProgrammeId] ON [dbo].[Galleries] ([ProgrammeId] ASC);
        PRINT N'[OK] Đã tạo bảng [Galleries].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [Galleries] đã tồn tại.';
    END

    -- ========================================================================
    -- 11. BẢNG Queries: CÂU HỎI & YÊU CẦU HỖ TRỢ
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Queries]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[Queries] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [UserId] UNIQUEIDENTIFIER NULL,
            [FullName] NVARCHAR(150) NOT NULL,
            [Email] NVARCHAR(256) NOT NULL,
            [Phone] NVARCHAR(50) NULL,
            [Subject] NVARCHAR(250) NOT NULL,
            [Message] NVARCHAR(MAX) NOT NULL,
            [Status] NVARCHAR(50) NOT NULL DEFAULT N'Open',
            [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            [ResolvedAt] DATETIME2(7) NULL,
            CONSTRAINT [PK_Queries] PRIMARY KEY CLUSTERED ([Id] ASC),
            CONSTRAINT [FK_Queries_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE SET NULL,
            CONSTRAINT [CK_Queries_Status] CHECK ([Status] IN (N'Open', N'InProgress', N'Resolved'))
        );

        CREATE NONCLUSTERED INDEX [IX_Queries_Status_CreatedAt] ON [dbo].[Queries] ([Status] ASC, [CreatedAt] DESC);
        CREATE NONCLUSTERED INDEX [IX_Queries_UserId] ON [dbo].[Queries] ([UserId] ASC);
        PRINT N'[OK] Đã tạo bảng [Queries].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [Queries] đã tồn tại.';
    END

    -- ========================================================================
    -- 12. BẢNG QueryReplies: LỊCH SỬ TRAO ĐỔI VÀ PHẢN HỒI
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryReplies]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[QueryReplies] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [QueryId] INT NOT NULL,
            [ResponderUserId] UNIQUEIDENTIFIER NOT NULL,
            [ReplyMessage] NVARCHAR(MAX) NOT NULL,
            [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            CONSTRAINT [PK_QueryReplies] PRIMARY KEY CLUSTERED ([Id] ASC),
            CONSTRAINT [FK_QueryReplies_Queries_QueryId] FOREIGN KEY ([QueryId]) REFERENCES [dbo].[Queries] ([Id]) ON DELETE CASCADE,
            CONSTRAINT [FK_QueryReplies_AspNetUsers_ResponderUserId] FOREIGN KEY ([ResponderUserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
        );

        CREATE NONCLUSTERED INDEX [IX_QueryReplies_QueryId] ON [dbo].[QueryReplies] ([QueryId] ASC);
        PRINT N'[OK] Đã tạo bảng [QueryReplies].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [QueryReplies] đã tồn tại.';
    END

    -- ========================================================================
    -- 13. BẢNG EmailInvitations: NHẬT KÝ MỜI BẠN BÈ QUA EMAIL
    -- ========================================================================
    IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailInvitations]') AND type = 'U')
    BEGIN
        CREATE TABLE [dbo].[EmailInvitations] (
            [Id] INT IDENTITY(1,1) NOT NULL,
            [SenderUserId] UNIQUEIDENTIFIER NULL,
            [SenderName] NVARCHAR(150) NOT NULL,
            [SenderEmail] NVARCHAR(256) NOT NULL,
            [RecipientEmail] NVARCHAR(256) NOT NULL,
            [Message] NVARCHAR(1000) NULL,
            [Status] NVARCHAR(50) NOT NULL DEFAULT N'Sent',
            [SentAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
            CONSTRAINT [PK_EmailInvitations] PRIMARY KEY CLUSTERED ([Id] ASC),
            CONSTRAINT [FK_EmailInvitations_AspNetUsers_SenderUserId] FOREIGN KEY ([SenderUserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE SET NULL,
            CONSTRAINT [CK_EmailInvitations_Status] CHECK ([Status] IN (N'Sent', N'Failed'))
        );

        CREATE NONCLUSTERED INDEX [IX_EmailInvitations_SenderUserId] ON [dbo].[EmailInvitations] ([SenderUserId] ASC);
        PRINT N'[OK] Đã tạo bảng [EmailInvitations].';
    END
    ELSE
    BEGIN
        PRINT N'[SKIP] Bảng [EmailInvitations] đã tồn tại.';
    END

    -- ========================================================================
    -- 14. BỔ SUNG PERMISSIONS CHO GIVE-AID VÀO BẢNG Permissions
    -- ========================================================================
    DECLARE @GiveAidPermissions TABLE (
        [Code] NVARCHAR(100),
        [Name] NVARCHAR(200),
        [GroupName] NVARCHAR(100),
        [Description] NVARCHAR(500)
    );

    INSERT INTO @GiveAidPermissions ([Code], [Name], [GroupName], [Description]) VALUES
    ('Donations.View', N'Xem lịch sử quyên góp', N'Quản lý Quyên góp', N'Cho phép xem danh sách và chi tiết các khoản quyên góp'),
    ('Donations.Manage', N'Quản lý giao dịch quyên góp', N'Quản lý Quyên góp', N'Cho phép cập nhật trạng thái giao dịch'),
    ('Causes.Manage', N'Quản lý danh mục quyên góp', N'Quản lý Quyên góp', N'Cho phép tạo, sửa, ẩn danh mục quyên góp'),
    ('Ngos.Manage', N'Quản lý tổ chức NGO', N'Quản lý NGO & Đối tác', N'Cho phép thêm mới và chỉnh sửa thông tin NGO'),
    ('Programmes.Manage', N'Quản lý chương trình sự kiện', N'Quản lý Chương trình', N'Cho phép tạo và cập nhật hoạt động của NGO'),
    ('Programmes.ViewRegistrations', N'Xem danh sách đăng ký tham gia', N'Quản lý Chương trình', N'Cho phép xem người dùng đăng ký tham gia sự kiện'),
    ('Content.Manage', N'Quản trị nội dung website', N'Quản trị Nội dung', N'Cho phép sửa nội dung About Us, Gallery, Đối tác'),
    ('Queries.Manage', N'Quản lý và trả lời câu hỏi', N'Hỗ trợ khách hàng', N'Cho phép xem và phản hồi các thắc mắc của người dùng');

    MERGE [dbo].[Permissions] AS Target
    USING @GiveAidPermissions AS Source
    ON (Target.[Code] = Source.[Code])
    WHEN MATCHED THEN
        UPDATE SET 
            Target.[Name] = Source.[Name],
            Target.[GroupName] = Source.[GroupName],
            Target.[Description] = Source.[Description]
    WHEN NOT MATCHED THEN
        INSERT ([Code], [Name], [GroupName], [Description], [CreatedAt])
        VALUES (Source.[Code], Source.[Name], Source.[GroupName], Source.[Description], SYSUTCDATETIME());

    PRINT N'[OK] Đã cập nhật danh mục quyền Give-AID trong bảng [Permissions].';

    -- Tự động gán quyền mới này cho vai trò Admin
    DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'ADMIN');
    IF (@AdminRoleId IS NOT NULL)
    BEGIN
        INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId], [CreatedAt])
        SELECT @AdminRoleId, p.[Id], SYSUTCDATETIME()
        FROM [dbo].[Permissions] p
        WHERE p.[Code] IN (SELECT [Code] FROM @GiveAidPermissions)
          AND NOT EXISTS (
              SELECT 1 FROM [dbo].[RolePermissions] rp 
              WHERE rp.[RoleId] = @AdminRoleId AND rp.[PermissionId] = p.[Id]
          );
        PRINT N'[OK] Đã gán quyền Give-AID cho vai trò Admin.';
    END

    PRINT N'>>> HOÀN TẤT CÁC BẢNG VÀ RÀNG BUỘC THÀNH CÔNG!';
    COMMIT TRANSACTION;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'!!! LỖI TRONG QUÁ TRÌNH MIGRATION: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
GO
