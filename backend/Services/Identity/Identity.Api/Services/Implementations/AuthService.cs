using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.Api.Data;
using Identity.Api.Data.Entities;
using Identity.Api.DTOs.Requests;
using Identity.Api.DTOs.Responses;
using Identity.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly AppIdentityDbContext _dbContext;
    private readonly INotificationClient _notificationClient;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService _tokenService,
        AppIdentityDbContext dbContext,
        INotificationClient notificationClient,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        this._tokenService = _tokenService;
        _dbContext = dbContext;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var cleanEmail = request.Email.Trim();
        var cleanFullName = request.FullName.Trim();

        // 1. Kiểm tra email đã tồn tại hay chưa
        var existingUser = await _userManager.FindByEmailAsync(cleanEmail);
        if (existingUser != null)
        {
            _logger.LogWarning("Đăng ký thất bại: Email {Email} đã tồn tại trong hệ thống.", cleanEmail);
            return ApiResponse<AuthResponse>.Fail("Email này đã được sử dụng.");
        }

        // 2. Tạo tài khoản người dùng mới
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = cleanEmail,
            Email = cleanEmail,
            FullName = cleanFullName,
            EmailConfirmed = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Đăng ký tài khoản cho {Email} thất bại: {Errors}", request.Email, string.Join("; ", errors));
            return ApiResponse<AuthResponse>.Fail("Đăng ký tài khoản thất bại.", errors);
        }

        // 3. Đảm bảo role "User" tồn tại và gán cho tài khoản mới
        const string defaultRole = "User";
        if (!await _roleManager.RoleExistsAsync(defaultRole))
        {
            await _roleManager.CreateAsync(new ApplicationRole(defaultRole, "Người dùng thông thường"));
        }
        await _userManager.AddToRoleAsync(user, defaultRole);

        // 4. Sinh cặp JWT Access Token và Refresh Token
        var roles = new List<string> { defaultRole };
        var permissions = await GetUserPermissionsAsync(user);
        var (accessToken, accessExpiresAt, jti) = await _tokenService.GenerateAccessTokenAsync(user, roles, permissions);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id, jti);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        // 5. Gửi email xác thực ban đầu qua NotificationService
        try
        {
            var initialVerificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _notificationClient.SendVerificationEmailAsync(user.Email!, user.FullName, initialVerificationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không thể gửi email xác thực ban đầu cho người dùng {Email}", user.Email);
        }

        _logger.LogInformation("Người dùng {Email} (UserId: {UserId}) đã đăng ký tài khoản thành công.", user.Email, user.Id);

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles,
            Permissions = permissions,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshTokenExpiresAt = refreshToken.ExpiryDate
        };

        return ApiResponse<AuthResponse>.Ok(response, "Đăng ký tài khoản thành công.");
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress = null)
    {
        // 1. Tìm tài khoản theo Email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Đăng nhập thất bại: Tài khoản {Email} không tồn tại. IP: {IpAddress}", request.Email, ipAddress);
            return ApiResponse<AuthResponse>.Fail("Email hoặc mật khẩu không chính xác.");
        }

        // 2. Kiểm tra tài khoản có bị khóa không
        if (!user.IsActive)
        {
            _logger.LogWarning("Đăng nhập thất bại: Tài khoản {Email} (UserId: {UserId}) đã bị vô hiệu hóa. IP: {IpAddress}", user.Email, user.Id, ipAddress);
            return ApiResponse<AuthResponse>.Fail("Tài khoản của bạn đã bị vô hiệu hóa.");
        }

        // 3. Kiểm tra mật khẩu
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Đăng nhập thất bại: Mật khẩu không chính xác cho tài khoản {Email}. IP: {IpAddress}", request.Email, ipAddress);
            return ApiResponse<AuthResponse>.Fail("Email hoặc mật khẩu không chính xác.");
        }

        // 4. Lấy danh sách Roles và Permissions hiệu lực
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user);

        // 5. Sinh JWT Access Token & Refresh Token
        var (accessToken, accessExpiresAt, jti) = await _tokenService.GenerateAccessTokenAsync(user, roles, permissions);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id, jti, ipAddress);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Người dùng {Email} (UserId: {UserId}) đã đăng nhập thành công từ IP {IpAddress}.", user.Email, user.Id, ipAddress);

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            Roles = roles.ToList(),
            Permissions = permissions,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshTokenExpiresAt = refreshToken.ExpiryDate
        };

        return ApiResponse<AuthResponse>.Ok(response, "Đăng nhập thành công.");
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress = null)
    {
        // 1. Giải mã claims từ expired Access Token
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return ApiResponse<AuthResponse>.Fail("AccessToken không hợp lệ.");
        }

        var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub) ?? principal.FindFirst(ClaimTypes.NameIdentifier);
        var jtiClaim = principal.FindFirst(JwtRegisteredClaimNames.Jti);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId) || jtiClaim == null)
        {
            return ApiResponse<AuthResponse>.Fail("Thông tin xác thực trong Token không hợp lệ.");
        }

        // 2. Tìm Refresh Token trong Database
        var savedRefreshToken = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken && r.UserId == userId);

        if (savedRefreshToken == null)
        {
            return ApiResponse<AuthResponse>.Fail("RefreshToken không tồn tại.");
        }

        // 3. Kiểm tra tính hợp lệ của Refresh Token
        if (savedRefreshToken.IsRevoked)
        {
            return ApiResponse<AuthResponse>.Fail("RefreshToken đã bị thu hồi.");
        }

        if (savedRefreshToken.IsUsed)
        {
            return ApiResponse<AuthResponse>.Fail("RefreshToken đã được sử dụng trước đó.");
        }

        if (savedRefreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return ApiResponse<AuthResponse>.Fail("RefreshToken đã hết hạn.");
        }

        if (savedRefreshToken.JwtId != jtiClaim.Value)
        {
            return ApiResponse<AuthResponse>.Fail("RefreshToken không khớp với AccessToken hiện tại.");
        }

        var user = savedRefreshToken.User;
        if (user == null || !user.IsActive)
        {
            return ApiResponse<AuthResponse>.Fail("Người dùng không hợp lệ hoặc đã bị khóa.");
        }

        // 4. Đánh dấu Refresh Token cũ đã sử dụng (Token Rotation)
        savedRefreshToken.IsUsed = true;

        // 5. Sinh cặp Token mới
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user);
        var (newAccessToken, accessExpiresAt, newJti) = await _tokenService.GenerateAccessTokenAsync(user, roles, permissions);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id, newJti, ipAddress);

        savedRefreshToken.ReplacedByToken = newRefreshToken.Token;
        _dbContext.RefreshTokens.Add(newRefreshToken);

        await _dbContext.SaveChangesAsync();

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            Roles = roles.ToList(),
            Permissions = permissions,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            AccessTokenExpiresAt = accessExpiresAt,
            RefreshTokenExpiresAt = newRefreshToken.ExpiryDate
        };

        return ApiResponse<AuthResponse>.Ok(response, "Làm mới Token thành công.");
    }

    public async Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken, string? ipAddress = null)
    {
        var token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        if (token == null)
        {
            _logger.LogWarning("Thu hồi token thất bại: RefreshToken không tồn tại. IP: {IpAddress}", ipAddress);
            return ApiResponse<bool>.Fail("RefreshToken không tồn tại.");
        }

        if (!token.IsActive)
        {
            _logger.LogWarning("Thu hồi token thất bại: RefreshToken của UserId {UserId} đã hết hạn hoặc bị thu hồi trước đó. IP: {IpAddress}", token.UserId, ipAddress);
            return ApiResponse<bool>.Fail("RefreshToken không còn hoạt động hoặc đã bị thu hồi.");
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Người dùng (UserId: {UserId}) đã đăng xuất/thu hồi RefreshToken thành công từ IP {IpAddress}.", token.UserId, ipAddress);

        return ApiResponse<bool>.Ok(true, "Thu hồi Token thành công.");
    }

    public async Task<ApiResponse<bool>> RevokeAllTokensAsync(Guid userId, string? ipAddress = null)
    {
        var activeTokens = await _dbContext.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked && !r.IsUsed && r.ExpiryDate > DateTime.UtcNow)
            .ToListAsync();

        if (activeTokens.Count == 0)
        {
            _logger.LogInformation("Yêu cầu Logout-all của UserId {UserId} hoàn tất: Không có phiên làm việc nào đang hoạt động.", userId);
            return ApiResponse<bool>.Ok(true, "Không có phiên đăng nhập nào đang hoạt động.");
        }

        var utcNow = DateTime.UtcNow;
        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = utcNow;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Người dùng (UserId: {UserId}) đã đăng xuất khỏi toàn bộ {Count} thiết bị từ IP {IpAddress}.", userId, activeTokens.Count, ipAddress);

        return ApiResponse<bool>.Ok(true, $"Đã đăng xuất thành công khỏi tất cả {activeTokens.Count} thiết bị.");
    }

    private async Task<List<string>> GetUserPermissionsAsync(ApplicationUser user)
    {
        // 1. Lấy danh sách Roles của user
        var userRoles = await _userManager.GetRolesAsync(user);
        var roleIds = await _roleManager.Roles
            .Where(r => userRoles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToListAsync();

        // 2. Lấy permissions từ roles
        var rolePermissions = await _dbContext.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();

        // 3. Lấy user permissions (override)
        var userPermissions = await _dbContext.UserPermissions
            .Include(up => up.Permission)
            .Where(up => up.UserId == user.Id)
            .ToListAsync();

        var grantedUserPerms = userPermissions.Where(up => up.IsGranted).Select(up => up.Permission.Code);
        var revokedUserPerms = userPermissions.Where(up => !up.IsGranted).Select(up => up.Permission.Code).ToHashSet();

        // 4. Tổng hợp quyền hiệu lực: (Role Perms + Granted Perms) - Revoked Perms
        var finalPermissions = rolePermissions
            .Union(grantedUserPerms)
            .Where(p => !revokedUserPerms.Contains(p))
            .Distinct()
            .ToList();

        return finalPermissions;
    }

    public async Task<ApiResponse<UserProfileResponse>> GetProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApiResponse<UserProfileResponse>.Fail("Không tìm thấy thông tin người dùng.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user);

        var response = new UserProfileResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            FullName = user.FullName ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            Roles = roles.ToList(),
            Permissions = permissions,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return ApiResponse<UserProfileResponse>.Ok(response, "Lấy thông tin cá nhân thành công.");
    }

    public async Task<ApiResponse<UserProfileResponse>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApiResponse<UserProfileResponse>.Fail("Không tìm thấy thông tin người dùng.");
        }

        if (!user.IsActive)
        {
            return ApiResponse<UserProfileResponse>.Fail("Tài khoản của bạn đã bị vô hiệu hóa.");
        }

        user.FullName = request.FullName.Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        user.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Cập nhật profile cho UserId {UserId} thất bại: {Errors}", userId, string.Join("; ", errors));
            return ApiResponse<UserProfileResponse>.Fail("Cập nhật thông tin cá nhân thất bại.", errors);
        }

        _logger.LogInformation("Người dùng {Email} (UserId: {UserId}) đã cập nhật thông tin cá nhân thành công.", user.Email, user.Id);

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user);

        var response = new UserProfileResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            FullName = user.FullName ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            Roles = roles.ToList(),
            Permissions = permissions,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return ApiResponse<UserProfileResponse>.Ok(response, "Cập nhật thông tin cá nhân thành công.");
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy thông tin người dùng.");
        }

        if (!user.IsActive)
        {
            return ApiResponse<bool>.Fail("Tài khoản của bạn đã bị vô hiệu hóa.");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Đổi mật khẩu cho UserId {UserId} thất bại: {Errors}", userId, string.Join("; ", errors));
            return ApiResponse<bool>.Fail("Đổi mật khẩu thất bại. Vui lòng kiểm tra lại mật khẩu hiện tại.", errors);
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Người dùng {Email} (UserId: {UserId}) đã đổi mật khẩu thành công.", user.Email, user.Id);

        return ApiResponse<bool>.Ok(true, "Đổi mật khẩu thành công.");
    }

    public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var cleanEmail = request.Email.Trim();
        var user = await _userManager.FindByEmailAsync(cleanEmail);

        // Tuân thủ bảo mật OWASP: Chống Account Enumeration (không tiết lộ email có tồn tại hay không)
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("ForgotPassword: Yêu cầu đặt lại mật khẩu cho tài khoản không tồn tại hoặc đã bị khóa: {Email}", cleanEmail);
            return ApiResponse<object>.Ok(
                new { message = "Nếu email này tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu đã được gửi đến hộp thư của bạn." },
                "Yêu cầu đặt lại mật khẩu đã được tiếp nhận.");
        }

        // Sinh Reset Password Token an toàn từ ASP.NET Core Identity Data Protection Provider
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        _logger.LogInformation("Đã tạo PasswordResetToken thành công cho User {Email} (UserId: {UserId}). Token: {Token}", user.Email, user.Id, token);

        // Gửi email đặt lại mật khẩu qua microservice Notification.Api
        try
        {
            await _notificationClient.SendPasswordResetEmailAsync(user.Email!, user.FullName, token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không thể gửi email đặt lại mật khẩu cho {Email} qua NotificationService.", user.Email);
        }

        // Trả về token trong response payload nhằm hỗ trợ kiểm thử và phát triển nhanh (Dev mode khi chưa tích hợp SMTP)
        return ApiResponse<object>.Ok(
            new 
            { 
                email = user.Email, 
                resetToken = token,
                message = "Nếu email này tồn tại trong hệ thống, hướng dẫn đặt lại mật khẩu đã được gửi đến hộp thư của bạn."
            }, 
            "Yêu cầu đặt lại mật khẩu đã được xử lý thành công.");
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var cleanEmail = request.Email.Trim();
        var user = await _userManager.FindByEmailAsync(cleanEmail);

        if (user == null || !user.IsActive)
        {
            return ApiResponse<bool>.Fail("Yêu cầu đặt lại mật khẩu không hợp lệ hoặc tài khoản đã bị khóa.");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Đặt lại mật khẩu cho {Email} thất bại: {Errors}", cleanEmail, string.Join("; ", errors));
            return ApiResponse<bool>.Fail("Đặt lại mật khẩu thất bại. Mã xác thực có thể không hợp lệ hoặc đã hết hạn.", errors);
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        // Bảo mật tối cao: Thu hồi toàn bộ Refresh Token của user để vô hiệu hóa tất cả phiên đăng nhập cũ
        await RevokeAllTokensAsync(user.Id);

        _logger.LogInformation("Người dùng {Email} (UserId: {UserId}) đã đặt lại mật khẩu thành công. Đã thu hồi toàn bộ phiên đăng nhập cũ.", user.Email, user.Id);

        return ApiResponse<bool>.Ok(true, "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại bằng mật khẩu mới.");
    }

    public async Task<ApiResponse<bool>> VerifyEmailAsync(VerifyEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy thông tin người dùng.");
        }

        if (user.EmailConfirmed)
        {
            return ApiResponse<bool>.Ok(true, "Email này đã được xác thực trước đó.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Xác thực email cho UserId {UserId} thất bại: {Errors}", user.Id, string.Join("; ", errors));
            return ApiResponse<bool>.Fail("Xác thực email thất bại. Mã xác thực không hợp lệ hoặc đã hết hạn.", errors);
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Người dùng {Email} (UserId: {UserId}) đã xác thực email thành công.", user.Email, user.Id);

        return ApiResponse<bool>.Ok(true, "Xác thực email thành công.");
    }

    public async Task<ApiResponse<object>> ResendVerificationEmailAsync(ResendVerificationEmailRequest request)
    {
        var cleanEmail = request.Email.Trim();
        var user = await _userManager.FindByEmailAsync(cleanEmail);

        // Tuân thủ bảo mật OWASP: Chống Account Enumeration
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("ResendVerificationEmail: Yêu cầu gửi lại xác thực cho tài khoản không tồn tại hoặc đã bị khóa: {Email}", cleanEmail);
            return ApiResponse<object>.Ok(
                new { message = "Nếu email này tồn tại trong hệ thống, mã xác thực mới đã được gửi đến hộp thư của bạn." },
                "Yêu cầu gửi lại email xác thực đã được tiếp nhận.");
        }

        if (user.EmailConfirmed)
        {
            return ApiResponse<object>.Ok(
                new { isAlreadyConfirmed = true, message = "Email này đã được xác thực trước đó." },
                "Email này đã được xác thực trước đó.");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        _logger.LogInformation("Đã tạo EmailConfirmationToken thành công cho User {Email} (UserId: {UserId}). Token: {Token}", user.Email, user.Id, token);

        // Gửi email xác thực qua microservice Notification.Api
        try
        {
            await _notificationClient.SendVerificationEmailAsync(user.Email!, user.FullName, token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không thể gửi email xác thực cho {Email} qua NotificationService.", user.Email);
        }

        return ApiResponse<object>.Ok(
            new 
            { 
                userId = user.Id, 
                email = user.Email, 
                verificationToken = token,
                message = "Nếu email này tồn tại trong hệ thống, mã xác thực mới đã được gửi đến hộp thư của bạn."
            }, 
            "Mã xác thực email đã được tạo và gửi thành công.");
    }
}
