using Identity.Api.DTOs.Requests;
using Identity.Api.DTOs.Responses;

namespace Identity.Api.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress = null);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress = null);
    Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken, string? ipAddress = null);
}
