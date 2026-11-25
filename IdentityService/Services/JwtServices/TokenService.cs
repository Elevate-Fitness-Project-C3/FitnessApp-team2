using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.Features.Login.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Services.JwtServices;

public sealed class TokenService(JwtOptions options) : ITokenService
{
    public LoginSessionDto GenerateTokens(
        string userId, string email, IEnumerable<string> roles, DateTimeOffset? refreshExpiresAtUtc = null)
    {
        // JWT time claims have whole-second precision.
        var now = DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        var accessExpiresAt = now.AddMinutes(options.DurationInMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };
        claims.AddRange(roles.Select(role => new Claim("roles", role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var jwt = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: accessExpiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new LoginSessionDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            AccessTokenExpiresAtUtc = accessExpiresAt,
            RefreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32)),
            RefreshTokenExpiresAtUtc = refreshExpiresAtUtc ?? now.AddDays(options.RefreshDurationInDays)
        };
    }
}
