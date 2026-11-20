using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityService.Features.Login.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Services.JwtServices;

public sealed class TokenService(
    JwtOptions options, TimeProvider clock, UserManager<ApplicationUser> users,
    IGenericRepository<RefreshSession> sessions) : ITokenService
{
    public async Task<LoginSessionViewModel?> CreateSessionAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await users.FindByIdAsync(userId);
        if (user is not { IsEnabled: true, EmailConfirmed: true })
            return null;

        var credentials = GenerateTokens(user.Id, user.Email!, await users.GetRolesAsync(user));
        await sessions.AddAsync(new RefreshSession
        {
            UserId = user.Id,
            TokenHash = RefreshSession.HashToken(credentials.RefreshToken),
            ExpiresAtUtc = credentials.RefreshTokenExpiresAtUtc
        }, cancellationToken);
        return credentials;
    }

    public async Task<LoginSessionViewModel?> RefreshSessionAsync(string? refreshToken, CancellationToken cancellationToken)
    {
        if (refreshToken is not { Length: 43 })
            return null;

        var hash = RefreshSession.HashToken(refreshToken);
        var validSession = sessions.Get(s => s.TokenHash == hash && s.ExpiresAtUtc > clock.GetUtcNow()
            && s.User.EmailConfirmed && s.User.IsEnabled);
        var session = await validSession.Include(s => s.User).SingleOrDefaultAsync(cancellationToken);
        if (session is null)
            return null;

        var roles = await users.GetRolesAsync(session.User);
        var credentials = GenerateTokens(session.UserId, session.User.Email!, roles, session.ExpiresAtUtc);
        var newHash = RefreshSession.HashToken(credentials.RefreshToken);
        // Only the request that replaces the old hash may return the new credentials.
        var updated = await validSession.ExecuteUpdateAsync(
            update => update.SetProperty(s => s.TokenHash, newHash), cancellationToken);
        return updated == 1 ? credentials : null;
    }

    private LoginSessionViewModel GenerateTokens(
        string userId, string email, IEnumerable<string> roles, DateTimeOffset? refreshExpiresAtUtc = null)
    {
        // JWT time claims have whole-second precision.
        var now = DateTimeOffset.FromUnixTimeSeconds(clock.GetUtcNow().ToUnixTimeSeconds());
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

        return new LoginSessionViewModel
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            AccessTokenExpiresAtUtc = accessExpiresAt,
            RefreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32)),
            RefreshTokenExpiresAtUtc = refreshExpiresAtUtc ?? now.AddDays(options.RefreshDurationInDays)
        };
    }
}
