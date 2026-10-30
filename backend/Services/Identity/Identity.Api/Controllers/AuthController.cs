using System.Security.Claims;
using Identity.Api.DTOs.Requests;
using Identity.Api.DTOs.Responses;
using Identity.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(request, ipAddress);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RefreshTokenAsync(request, ipAddress);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RevokeTokenAsync(request.RefreshToken, ipAddress);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(ApiResponse<bool>.Ok(true, "Đăng xuất thành công."));
    }

    [HttpPost("logout-all")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAll()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<bool>.Fail("Không xác định được danh tính người dùng."));
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RevokeAllTokensAsync(userId, ipAddress);

        return Ok(result);
    }

    [HttpPost("revoke-token")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] LogoutRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RevokeTokenAsync(request.RefreshToken, ipAddress);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<UserProfileResponse>.Fail("Không xác định được danh tính người dùng."));
        }

        var result = await _authService.GetProfileAsync(userId);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<UserProfileResponse>.Fail("Không xác định được danh tính người dùng."));
        }

        var result = await _authService.UpdateProfileAsync(userId, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(ApiResponse<bool>.Fail("Không xác định được danh tính người dùng."));
        }

        var result = await _authService.ChangePasswordAsync(userId, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        var result = await _authService.VerifyEmailAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("resend-verification-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailRequest request)
    {
        var result = await _authService.ResendVerificationEmailAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
