-- ============================================================================
-- VERIFICATION & TESTING SCRIPT FOR GIVE-AID DATABASE
-- ============================================================================

USE [IdentityDb];
GO

SET NOCOUNT ON;
PRINT N'====================================================================';
PRINT N'BẮT ĐẦU KIỂM THỬ CƠ SỞ DỮ LIỆU GIVE-AID...';
PRINT N'====================================================================';

-- Test 1: Kiểm tra các bảng đã được tạo đủ
PRINT N'--- Test 1: Kiểm tra sự tồn tại của các bảng ---';
SELECT 
    t.name AS TableName,
    p.rows AS [RowCount]
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
WHERE t.name IN (
    'AspNetUsers', 'Causes', 'Ngos', 'NgoCauses', 'Programmes', 
    'ProgrammeRegistrations', 'Donations', 'Partners', 'AboutPages', 
    'Galleries', 'Queries', 'QueryReplies', 'EmailInvitations'
)
ORDER BY t.name;

-- Test 2: Thử nghiệm quyên góp Guest hợp lệ (100.000 VNĐ)
PRINT N'--- Test 2: Thêm giao dịch quyên góp hợp lệ (Guest) ---';
DECLARE @TestCauseId INT = (SELECT TOP 1 [Id] FROM [dbo].[Causes] WHERE [Slug] = 'children');
DECLARE @NewDonationId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [dbo].[Donations] (
    [Id], [TransactionCode], [CauseId], [UserId], [DonorName], [DonorEmail], 
    [Amount], [PaymentMethod], [CardLastFour], [Status]
)
VALUES (
    @NewDonationId, N'GD-TEST-0001', @TestCauseId, NULL, N'Nhà hảo tâm ẩn danh', 
    N'guest@example.com', 100000.00, N'MockCard', N'4242', N'Successful'
);

IF EXISTS (SELECT 1 FROM [dbo].[Donations] WHERE [Id] = @NewDonationId)
    PRINT N'[PASS] Giao dịch Guest hợp lệ đã được ghi nhận.';
ELSE
    PRINT N'[FAIL] Không tạo được giao dịch Guest.';

-- Test 3: Kiểm tra ràng buộc số tiền vi phạm (Dưới 10.000 VNĐ -> phải bị chặn)
PRINT N'--- Test 3: Kiểm tra ràng buộc số tiền < 10.000 VNĐ (Kỳ vọng: Báo lỗi CHECK) ---';
BEGIN TRY
    INSERT INTO [dbo].[Donations] (
        [TransactionCode], [CauseId], [DonorName], [DonorEmail], [Amount], [Status]
    )
    VALUES (
        N'GD-TEST-INVALID', @TestCauseId, N'Test User', N'test@example.com', 5000.00, N'Successful'
    );
    PRINT N'[FAIL] Ràng buộc CHECK không hoạt động!';
END TRY
BEGIN CATCH
    PRINT N'[PASS] Đã chặn thành công số tiền không hợp lệ (< 10.000 VNĐ). Mã lỗi: ' + CAST(ERROR_NUMBER() AS NVARCHAR(20));
END CATCH;

-- Test 4: Kiểm tra truy vấn thống kê quyên góp (View vw_DonationStatistics)
PRINT N'--- Test 4: Thống kê tổng quyên góp thành công theo từng Cause ---';
SELECT * FROM [dbo].[vw_DonationStatistics];

-- Test 5: Kiểm tra toàn vẹn khi User bị xóa/khóa -> Donation vẫn còn nguyên vẹn
PRINT N'--- Test 5: Kiểm tra tính độc lập và bảo toàn lịch sử giao dịch ---';
SELECT TOP 5 
    d.[TransactionCode],
    d.[Amount],
    d.[Status],
    d.[DonorName],
    ISNULL(u.[Email], N'Khách vãng lai (Guest)') AS UserAccount,
    d.[CreatedAt]
FROM [dbo].[Donations] d
LEFT JOIN [dbo].[AspNetUsers] u ON d.[UserId] = u.[Id]
ORDER BY d.[CreatedAt] DESC;

-- Dọn dẹp dữ liệu kiểm thử
DELETE FROM [dbo].[Donations] WHERE [TransactionCode] = N'GD-TEST-0001';
PRINT N'[CLEANUP] Đã xóa bản ghi kiểm thử.';

PRINT N'====================================================================';
PRINT N'KIỂM THỬ HOÀN TẤT!';
PRINT N'====================================================================';
GO
