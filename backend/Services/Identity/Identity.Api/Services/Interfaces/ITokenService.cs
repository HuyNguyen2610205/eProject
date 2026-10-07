using System.Security.Claims;
using Identity.Api.Data.Entities;

namespace Identity.Api.Services.Interfaces;

public interface ITokenService
{
    Task<(string accessToken, DateTime expiresAt, string jti)> GenerateAccessTokenAsync(ApplicationUser user, IList<string> roles);
    RefreshToken GenerateRefreshToken(Guid userId, string jwtId, string? ipAddress = null);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
