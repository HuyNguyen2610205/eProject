-- ============================================================================
-- DATABASE INITIALIZATION SCRIPT FOR IDENTITY SERVICE (ASP.NET CORE IDENTITY)
-- Project: eProject
-- Compatible with: SQL Server 2019 / 2022 / Azure SQL
-- Primary Key Strategy: UNIQUEIDENTIFIER (GUID)
-- ============================================================================

-- 1. Create Database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'IdentityDb')
BEGIN
    CREATE DATABASE [IdentityDb];
    PRINT N'Database [IdentityDb] created successfully.';
END
GO

USE [IdentityDb];
GO

-- ============================================================================
-- 2. CREATE ASP.NET CORE IDENTITY TABLES
-- ============================================================================

-- Table: AspNetRoles
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AspNetRoles] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [Name] NVARCHAR(256) NULL,
        [NormalizedName] NVARCHAR(256) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        [Description] NVARCHAR(500) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] 
    ON [dbo].[AspNetRoles] ([NormalizedName] ASC) 
    WHERE ([NormalizedName] IS NOT NULL);

    PRINT N'Table [AspNetRoles] created.';
END
GO

-- Table: AspNetUsers (Customized with FullName, AvatarUrl, IsActive, CreatedAt, UpdatedAt)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AspNetUsers] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [UserName] NVARCHAR(256) NULL,
        [NormalizedUserName] NVARCHAR(256) NULL,
        [Email] NVARCHAR(256) NULL,
        [NormalizedEmail] NVARCHAR(256) NULL,
        [EmailConfirmed] BIT NOT NULL DEFAULT 0,
        [PasswordHash] NVARCHAR(MAX) NULL,
        [SecurityStamp] NVARCHAR(MAX) NULL,
        [ConcurrencyStamp] NVARCHAR(MAX) NULL,
        [PhoneNumber] NVARCHAR(50) NULL,
        [PhoneNumberConfirmed] BIT NOT NULL DEFAULT 0,
        [TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
        [LockoutEnd] DATETIMEOFFSET(7) NULL,
        [LockoutEnabled] BIT NOT NULL DEFAULT 1,
        [AccessFailedCount] INT NOT NULL DEFAULT 0,
        
        -- Custom fields for application
        [FullName] NVARCHAR(150) NULL,
        [AvatarUrl] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        [UpdatedAt] DATETIME2(7) NULL,

        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE NONCLUSTERED INDEX [EmailIndex] 
    ON [dbo].[AspNetUsers] ([NormalizedEmail] ASC);

    CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] 
    ON [dbo].[AspNetUsers] ([NormalizedUserName] ASC) 
    WHERE ([NormalizedUserName] IS NOT NULL);

    PRINT N'Table [AspNetUsers] created.';
END
GO

-- Table: AspNetRoleClaims
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoleClaims]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AspNetRoleClaims] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        [ClaimType] NVARCHAR(MAX) NULL,
        [ClaimValue] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) 
            REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] 
    ON [dbo].[AspNetRoleClaims] ([RoleId] ASC);

    PRINT N'Table [AspNetRoleClaims] created.';
END
GO

-- Table: AspNetUserClaims
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AspNetUserClaims] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [ClaimType] NVARCHAR(MAX) NULL,
        [ClaimValue] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] 
    ON [dbo].[AspNetUserClaims] ([UserId] ASC);

    PRINT N'Table [AspNetUserClaims] created.';
END
GO

-- Table: AspNetUserLogins (For external auth like Google, Facebook)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AspNetUserLogins] (
        [LoginProvider] NVARCHAR(128) NOT NULL,
        [ProviderKey] NVARCHAR(128) NOT NULL,
        [ProviderDisplayName] NVARCHAR(MAX) NULL,
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] 
    ON [dbo].[AspNetUserLogins] ([UserId] ASC);

    PRINT N'Table [AspNetUserLogins] created.';
END
GO

-- Table: AspNetUserRoles (Mapping User to Roles)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AspNetUserRoles] (
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [RoleId] UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) 
            REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] 
    ON [dbo].[AspNetUserRoles] ([RoleId] ASC);

    PRINT N'Table [AspNetUserRoles] created.';
END
GO

-- Table: AspNetUserTokens
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AspNetUserTokens] (
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [LoginProvider] NVARCHAR(128) NOT NULL,
        [Name] NVARCHAR(128) NOT NULL,
        [Value] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    PRINT N'Table [AspNetUserTokens] created.';
END
GO

-- ============================================================================
-- 3. TABLE FOR JWT REFRESH TOKENS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefreshTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RefreshTokens] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [Token] NVARCHAR(256) NOT NULL,
        [JwtId] NVARCHAR(128) NOT NULL,            -- Associated JTI in Access Token
        [IsUsed] BIT NOT NULL DEFAULT 0,            -- True if rotated/exchanged
        [IsRevoked] BIT NOT NULL DEFAULT 0,         -- True if manually invalidated (Logout)
        [ExpiryDate] DATETIME2(7) NOT NULL,         -- Refresh token expiration time
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        [CreatedByIp] NVARCHAR(50) NULL,
        [RevokedAt] DATETIME2(7) NULL,
        [ReplacedByToken] NVARCHAR(256) NULL,

        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_RefreshTokens_UserId] 
    ON [dbo].[RefreshTokens] ([UserId] ASC);

    CREATE UNIQUE NONCLUSTERED INDEX [IX_RefreshTokens_Token] 
    ON [dbo].[RefreshTokens] ([Token] ASC);

    PRINT N'Table [RefreshTokens] created.';
END
GO

-- ============================================================================
-- 4. SEED INITIAL DATA (Default Roles)
-- ============================================================================
-- Role: Admin
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'ADMIN')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp], [Description])
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID(), N'Quản trị viên hệ thống');
    PRINT N'Role [Admin] seeded.';
END

-- Role: User
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'USER')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp], [Description])
    VALUES (NEWID(), 'User', 'USER', NEWID(), N'Người dùng thông thường');
    PRINT N'Role [User] seeded.';
END

-- Role: Manager (Optional)
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'MANAGER')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp], [Description])
    VALUES (NEWID(), 'Manager', 'MANAGER', NEWID(), N'Quản lý nghiệp vụ');
    PRINT N'Role [Manager] seeded.';
END
GO

PRINT N'============================================================================';
PRINT N'SUCCESS: Identity database initialization script executed successfully!';
PRINT N'============================================================================';
GO
