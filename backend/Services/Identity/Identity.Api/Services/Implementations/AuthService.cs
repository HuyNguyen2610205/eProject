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
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService _tokenService,
        AppIdentityDbContext dbContext,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        this._tokenService = _tokenService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        // 1. Kiểm tra email đã tồn tại hay chưa
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return ApiResponse<AuthResponse>.Fail("Email này đã được sử dụng.");
        }

        // 2. Tạo tài khoản người dùng mới
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmailConfirmed = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
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
        var (accessToken, accessExpiresAt, jti) = await _tokenService.GenerateAccessTokenAsync(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id, jti);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles,
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
            return ApiResponse<AuthResponse>.Fail("Email hoặc mật khẩu không chính xác.");
        }

        // 2. Kiểm tra tài khoản có bị khóa không
        if (!user.IsActive)
        {
            return ApiResponse<AuthResponse>.Fail("Tài khoản của bạn đã bị vô hiệu hóa.");
        }

        // 3. Kiểm tra mật khẩu
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return ApiResponse<AuthResponse>.Fail("Email hoặc mật khẩu không chính xác.");
        }

        // 4. Lấy danh sách Roles
        var roles = await _userManager.GetRolesAsync(user);

        // 5. Sinh JWT Access Token & Refresh Token
        var (accessToken, accessExpiresAt, jti) = await _tokenService.GenerateAccessTokenAsync(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id, jti, ipAddress);

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            Roles = roles.ToList(),
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
        var (newAccessToken, accessExpiresAt, newJti) = await _tokenService.GenerateAccessTokenAsync(user, roles);
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
            return ApiResponse<bool>.Fail("RefreshToken không tồn tại.");
        }

        if (!token.IsActive)
        {
            return ApiResponse<bool>.Fail("RefreshToken không còn hoạt động hoặc đã bị thu hồi.");
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Thu hồi Token thành công.");
    }
}
