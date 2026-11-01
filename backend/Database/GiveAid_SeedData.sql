-- ============================================================================
-- SEED DATA SCRIPT FOR GIVE-AID NGO PLATFORM
-- Database: IdentityDb
-- ============================================================================

USE [IdentityDb];
GO

SET NOCOUNT ON;

PRINT N'>>> BẮT ĐẦU SEED DỮ LIỆU MẪU CHO GIVE-AID...';

-- 1. SEED 6 DANH MỤC QUYÊN GÓP CHÍNH (SRS FR-08 & Q-06)
MERGE [dbo].[Causes] AS Target
USING (VALUES
    (N'Trẻ em', N'children', N'Hỗ trợ trẻ em nghèo, cơ nhỡ, cung cấp dinh dưỡng và điều kiện sống an toàn.', N'/icons/children.svg', 1, 1),
    (N'Giáo dục', N'education', N'Trao học bổng, xây dựng trường lớp và cung cấp sách vở cho học sinh vùng cao.', N'/icons/education.svg', 1, 2),
    (N'Người khuyết tật', N'disabled', N'Tài trợ thiết bị phục hồi chức năng, tạo sinh kế và đào tạo nghề hòa nhập cộng đồng.', N'/icons/disabled.svg', 1, 3),
    (N'Phụ nữ', N'women', N'Hỗ trợ phụ nữ có hoàn cảnh khó khăn, thúc đẩy bình đẳng giới và tự chủ tài chính.', N'/icons/women.svg', 1, 4),
    (N'Thanh niên', N'youth', N'Đào tạo kỹ năng sống, ươm mầm sáng kiến cộng đồng cho thế hệ trẻ.', N'/icons/youth.svg', 1, 5),
    (N'Người cao tuổi', N'elderly', N'Chăm sóc y tế, cung cấp bữa ăn và nhà ở ấm cúng cho người già neo đơn.', N'/icons/elderly.svg', 1, 6)
) AS Source ([Name], [Slug], [Description], [IconUrl], [IsActive], [DisplayOrder])
ON Target.[Slug] = Source.[Slug]
WHEN NOT MATCHED THEN
    INSERT ([Name], [Slug], [Description], [IconUrl], [IsActive], [DisplayOrder], [CreatedAt])
    VALUES (Source.[Name], Source.[Slug], Source.[Description], Source.[IconUrl], Source.[IsActive], Source.[DisplayOrder], SYSUTCDATETIME());

PRINT N'[OK] Đã seed 6 danh mục quyên góp cơ bản.';

-- 2. SEED CÁC TỔ CHỨC PHI CHÍNH PHỦ (NGO) MẪU
IF NOT EXISTS (SELECT 1 FROM [dbo].[Ngos] WHERE [Name] = N'Quỹ Trò Nghèo Vùng Cao')
BEGIN
    INSERT INTO [dbo].[Ngos] ([Name], [Description], [LogoUrl], [Website], [ContactEmail], [Phone], [IsActive], [DisplayOrder])
    VALUES 
    (N'Quỹ Trò Nghèo Vùng Cao', N'Tổ chức thiện nguyện hỗ trợ cơm ăn, áo ấm và đồ dùng học tập cho học sinh vùng sâu vùng xa.', N'/logos/trongheo.png', N'https://trongheovungcao.vn', N'contact@trongheovungcao.vn', N'0243.999.888', 1, 1),
    (N'Tổ chức Trái Tim Cho Em', N'Hỗ trợ phẫu thuật tim nhân đạo cho các bệnh nhi nghèo có hoàn cảnh đặc biệt khó khăn.', N'/logos/traitimchoem.png', N'https://traitimchoem.vn', N'info@traitimchoem.vn', N'0243.777.666', 1, 2);

    PRINT N'[OK] Đã seed NGO mẫu.';
END

-- Gán NGO vào danh mục Causes
DECLARE @Ngo1Id INT = (SELECT TOP 1 [Id] FROM [dbo].[Ngos] WHERE [Name] = N'Quỹ Trò Nghèo Vùng Cao');
DECLARE @Ngo2Id INT = (SELECT TOP 1 [Id] FROM [dbo].[Ngos] WHERE [Name] = N'Tổ chức Trái Tim Cho Em');
DECLARE @CauseEduId INT = (SELECT TOP 1 [Id] FROM [dbo].[Causes] WHERE [Slug] = 'education');
DECLARE @CauseChildId INT = (SELECT TOP 1 [Id] FROM [dbo].[Causes] WHERE [Slug] = 'children');

IF (@Ngo1Id IS NOT NULL AND @CauseEduId IS NOT NULL)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[NgoCauses] WHERE [NgoId] = @Ngo1Id AND [CauseId] = @CauseEduId)
        INSERT INTO [dbo].[NgoCauses] ([NgoId], [CauseId]) VALUES (@Ngo1Id, @CauseEduId);
END

IF (@Ngo2Id IS NOT NULL AND @CauseChildId IS NOT NULL)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[NgoCauses] WHERE [NgoId] = @Ngo2Id AND [CauseId] = @CauseChildId)
        INSERT INTO [dbo].[NgoCauses] ([NgoId], [CauseId]) VALUES (@Ngo2Id, @CauseChildId);
END

-- 3. SEED ĐỐI TÁC (Partners)
MERGE [dbo].[Partners] AS Target
USING (VALUES
    (N'Tập đoàn Viễn thông VNPT', N'/logos/partners/vnpt.png', N'https://vnpt.com.vn', N'Đơn vị bảo trợ truyền thông', 1, 1),
    (N'Ngân hàng Vietcombank', N'/logos/partners/vcb.png', N'https://vietcombank.com.vn', N'Đối tác đồng hành các chương trình an sinh xã hội', 2, 1)
) AS Source ([Name], [LogoUrl], [Website], [Description], [DisplayOrder], [IsActive])
ON Target.[Name] = Source.[Name]
WHEN NOT MATCHED THEN
    INSERT ([Name], [LogoUrl], [Website], [Description], [DisplayOrder], [IsActive], [CreatedAt])
    VALUES (Source.[Name], Source.[LogoUrl], Source.[Website], Source.[Description], Source.[DisplayOrder], Source.[IsActive], SYSUTCDATETIME());

PRINT N'[OK] Đã seed Đối tác mẫu.';

-- 4. SEED 7 TRANG NỘI DUNG ABOUT US (SRS FR-04)
MERGE [dbo].[AboutPages] AS Target
USING (VALUES
    (N'what-we-do', N'Chúng Tôi Làm Gì (What We Do)', N'Give-AID là nền tảng kết nối trực tiếp các nhà hảo tâm với các tổ chức phi chính phủ uy tín, đảm bảo mọi khoản đóng góp đều được công khai và minh bạch.'),
    (N'our-mission', N'Sứ Mệnh Của Chúng Tôi (Our Mission)', N'Lan tỏa tinh thần tương thân tương ái, ứng dụng công nghệ để nâng cao hiệu quả và tính minh bạch của các hoạt động thiện nguyện tại Việt Nam.'),
    (N'our-team', N'Đội Ngũ Phát Triển (Our Team)', N'Đội ngũ gồm các chuyên gia công nghệ, các điều phối viên hoạt động xã hội tận tâm vì cộng đồng.'),
    (N'career-with-us', N'Cơ Hội Nghề Nghiệp (Career With Us)', N'Gia nhập Give-AID để cùng chúng tôi tạo nên những tác động tích cực và lâu dài cho xã hội.'),
    (N'our-achievements', N'Thành Tựu Đạt Được (Our Achievements)', N'Hơn 500+ chương trình được triển khai thành công, tiếp sức cho hàng triệu mảnh đời khó khăn trên khắp cả nước.'),
    (N'our-supporters', N'Nhà Hảo Tâm & Bảo Trợ (Our Supporters)', N'Lời tri ân sâu sắc gửi đến các cá nhân, tổ chức và doanh nghiệp đã luôn đồng hành cùng sứ mệnh của Give-AID.'),
    (N'read-about-us', N'Báo Chí Nói Về Chúng Tôi (Read About Us)', N'Tổng hợp các bài viết, phóng sự truyền thông ghi nhận hành trình hoạt động của Give-AID.')
) AS Source ([Slug], [Title], [Content])
ON Target.[Slug] = Source.[Slug]
WHEN NOT MATCHED THEN
    INSERT ([Slug], [Title], [Content], [IsPublished], [UpdatedAt])
    VALUES (Source.[Slug], Source.[Title], Source.[Content], 1, SYSUTCDATETIME());

PRINT N'[OK] Đã seed 7 trang nội dung About Us.';

-- 5. SEED CHƯƠNG TRÌNH MẪU
IF (@Ngo1Id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [dbo].[Programmes] WHERE [Title] = N'Áo Ấm Mùa Đông Cho Em 2026'))
BEGIN
    INSERT INTO [dbo].[Programmes] ([NgoId], [Title], [Description], [Category], [Location], [StartDate], [EndDate], [ImageUrl], [Status], [IsFeatured])
    VALUES 
    (@Ngo1Id, N'Áo Ấm Mùa Đông Cho Em 2026', N'Chiến dịch mang 5.000 chiếc áo ấm và đồ dùng học tập đến học sinh vùng cao Hà Giang.', N'Education', N'Huyện Mèo Vạc, Tỉnh Hà Giang', '2026-11-15', '2026-12-30', N'/programmes/aoam.jpg', N'Upcoming', 1);

    PRINT N'[OK] Đã seed Chương trình mẫu.';
END

PRINT N'>>> HOÀN TẤT SEED DỮ LIỆU MẪU THÀNH CÔNG!';
GO
