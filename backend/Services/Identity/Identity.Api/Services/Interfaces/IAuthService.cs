using Identity.Api.DTOs.Requests;
using Identity.Api.DTOs.Responses;

namespace Identity.Api.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress = null);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress = null);
    Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken, string? ipAddress = null);
    Task<ApiResponse<bool>> RevokeAllTokensAsync(Guid userId, string? ipAddress = null);
    Task<ApiResponse<UserProfileResponse>> GetProfileAsync(Guid userId);
    Task<ApiResponse<UserProfileResponse>> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request);
    Task<ApiResponse<bool>> VerifyEmailAsync(VerifyEmailRequest request);
    Task<ApiResponse<object>> ResendVerificationEmailAsync(ResendVerificationEmailRequest request);
}
