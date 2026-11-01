using Donation.Api.Data;
using Donation.Api.Data.Entities;
using Donation.Api.DTOs.Requests;
using Donation.Api.DTOs.Responses;
using Donation.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Donation.Api.Services.Implementations;

public class DonationService : IDonationService
{
    private readonly DonationDbContext _dbContext;
    private readonly INotificationClient _notificationClient;
    private readonly ILogger<DonationService> _logger;

    public DonationService(
        DonationDbContext dbContext,
        INotificationClient notificationClient,
        ILogger<DonationService> logger)
    {
        _dbContext = dbContext;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    public async Task<ApiResponse<DonationResponse>> ProcessDonationAsync(DonateRequest request, Guid? userId = null)
    {
        // 1. Kiểm tra danh mục quyên góp tồn tại và đang hoạt động
        var cause = await _dbContext.Causes.FirstOrDefaultAsync(c => c.Id == request.CauseId && c.IsActive);
        if (cause == null)
        {
            return ApiResponse<DonationResponse>.Fail("Danh mục quyên góp không tồn tại hoặc đã tạm dừng nhận đóng góp.");
        }

        // 2. Kiểm tra số tiền hợp lệ theo quy tắc Q-08 (10.000 VNĐ -> 100.000.000 VNĐ)
        if (request.Amount < 10000m || request.Amount > 100000000m)
        {
            return ApiResponse<DonationResponse>.Fail("Số tiền quyên góp phải nằm trong khoảng từ 10.000 VNĐ đến 100.000.000 VNĐ.");
        }

        // 3. Kiểm tra định dạng thẻ giả lập
        var cleanCardNumber = request.CardNumber.Replace(" ", "").Replace("-", "");
        if (cleanCardNumber.Length != 16 || !cleanCardNumber.All(char.IsDigit))
        {
            return ApiResponse<DonationResponse>.Fail("Số thẻ không hợp lệ. Vui lòng nhập thẻ giả lập gồm 16 chữ số.");
        }

        var cardLastFour = cleanCardNumber[^4..];

        // 4. Kịch bản mô phỏng giao dịch thanh toán (Q-02, FR-16)
        // Nếu chọn SimulateFailure hoặc thẻ kết thúc bằng 0000 thì mô phỏng thất bại
        var isPaymentSuccess = !request.SimulateFailure && cardLastFour != "0000";
        var transactionStatus = isPaymentSuccess ? "Successful" : "Failed";

        // 5. Sinh mã giao dịch duy nhất: GD-YYYYMMDD-XXXX
        var transactionCode = await GenerateUniqueTransactionCodeAsync();

        var donation = new Data.Entities.Donation
        {
            Id = Guid.NewGuid(),
            TransactionCode = transactionCode,
            CauseId = cause.Id,
            UserId = userId,
            DonorName = request.DonorName.Trim(),
            DonorEmail = request.DonorEmail.Trim().ToLowerInvariant(),
            DonorPhone = string.IsNullOrWhiteSpace(request.DonorPhone) ? null : request.DonorPhone.Trim(),
            Amount = request.Amount,
            PaymentMethod = "MockCard",
            CardLastFour = cardLastFour,
            Status = transactionStatus,
            Notes = request.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        _dbContext.Donations.Add(donation);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Đã xử lý quyên góp {Status}. Mã GD: {Code}, Số tiền: {Amount:N0} VNĐ, Người gửi: {Email}",
            donation.Status, donation.TransactionCode, donation.Amount, donation.DonorEmail);

        // 6. Gửi email xác nhận nếu giao dịch thành công (S2S qua NotificationService)
        if (isPaymentSuccess)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _notificationClient.SendDonationThankYouEmailAsync(
                        donation.DonorEmail,
                        donation.DonorName,
                        donation.TransactionCode,
                        donation.Amount,
                        cause.Name
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi ngầm khi gửi email cảm ơn quyên góp cho {Email}", donation.DonorEmail);
                }
            });
        }

        var response = MapToResponse(donation, cause);

        if (!isPaymentSuccess)
        {
            return ApiResponse<DonationResponse>.Fail("Thanh toán giả lập không thành công. Vui lòng kiểm tra lại thông tin thẻ kiểm thử hoặc thử lại sau.", 
                new List<string> { "Giao dịch mô phỏng bị từ chối bởi kịch bản kiểm thử thanh toán." });
        }

        return ApiResponse<DonationResponse>.Ok(response, "Quyên góp thành công! Cảm ơn tấm lòng hảo tâm của bạn.");
    }

    public async Task<ApiResponse<DonationResponse>> GetDonationByCodeAsync(string transactionCode)
    {
        var code = transactionCode.Trim().ToUpperInvariant();
        var donation = await _dbContext.Donations
            .AsNoTracking()
            .Include(d => d.Cause)
            .FirstOrDefaultAsync(d => d.TransactionCode == code);

        if (donation == null)
        {
            return ApiResponse<DonationResponse>.Fail("Không tìm thấy thông tin giao dịch với mã quyên góp đã cung cấp.");
        }

        return ApiResponse<DonationResponse>.Ok(MapToResponse(donation, donation.Cause));
    }

    public async Task<ApiResponse<List<DonationResponse>>> GetUserDonationHistoryAsync(Guid userId)
    {
        var donations = await _dbContext.Donations
            .AsNoTracking()
            .Include(d => d.Cause)
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => MapToResponse(d, d.Cause))
            .ToListAsync();

        return ApiResponse<List<DonationResponse>>.Ok(donations, "Lấy lịch sử quyên góp thành công.");
    }

    public async Task<ApiResponse<DonationSummaryResponse>> GetDonationSummaryAsync()
    {
        var statsView = await _dbContext.DonationStatistics.AsNoTracking().ToListAsync();

        var totalRaised = statsView.Sum(s => s.TotalAmountRaised);
        var totalCount = await _dbContext.Donations.CountAsync();
        var successCount = statsView.Sum(s => s.SuccessfulDonations);
        var failedCount = await _dbContext.Donations.CountAsync(d => d.Status == "Failed");
        var pendingCount = await _dbContext.Donations.CountAsync(d => d.Status == "Pending");

        var summary = new DonationSummaryResponse
        {
            TotalAmountRaised = totalRaised,
            TotalDonationsCount = totalCount,
            SuccessfulDonationsCount = successCount,
            PendingDonationsCount = pendingCount,
            FailedDonationsCount = failedCount,
            CausesStats = statsView.Select(s => new CauseStatisticItemResponse
            {
                CauseId = s.CauseId,
                CauseName = s.CauseName,
                CauseSlug = s.CauseSlug,
                TotalDonations = s.TotalDonations,
                SuccessfulDonations = s.SuccessfulDonations,
                TotalAmountRaised = s.TotalAmountRaised
            }).ToList()
        };

        return ApiResponse<DonationSummaryResponse>.Ok(summary);
    }

    public async Task<ApiResponse<List<DonationResponse>>> GetAllDonationsAsync(string? status = null, int? causeId = null, int page = 1, int pageSize = 20)
    {
        var query = _dbContext.Donations.AsNoTracking().Include(d => d.Cause).AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(d => d.Status == status.Trim());
        }

        if (causeId.HasValue && causeId.Value > 0)
        {
            query = query.Where(d => d.CauseId == causeId.Value);
        }

        var donations = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => MapToResponse(d, d.Cause))
            .ToListAsync();

        return ApiResponse<List<DonationResponse>>.Ok(donations);
    }

    private async Task<string> GenerateUniqueTransactionCodeAsync()
    {
        string code;
        bool exists;
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");

        do
        {
            var randomNum = RandomNumberGenerator.GetInt32(1000, 9999);
            code = $"GD-{datePart}-{randomNum}";
            exists = await _dbContext.Donations.AnyAsync(d => d.TransactionCode == code);
        } while (exists);

        return code;
    }

    private static DonationResponse MapToResponse(Data.Entities.Donation d, Cause? cause)
    {
        return new DonationResponse
        {
            Id = d.Id,
            TransactionCode = d.TransactionCode,
            CauseId = d.CauseId,
            CauseName = cause?.Name ?? string.Empty,
            CauseSlug = cause?.Slug ?? string.Empty,
            UserId = d.UserId,
            DonorName = d.DonorName,
            DonorEmail = d.DonorEmail,
            DonorPhone = d.DonorPhone,
            Amount = d.Amount,
            PaymentMethod = d.PaymentMethod,
            CardLastFour = d.CardLastFour,
            Status = d.Status,
            Notes = d.Notes,
            CreatedAt = d.CreatedAt,
            CompletedAt = d.CompletedAt
        };
    }
}
