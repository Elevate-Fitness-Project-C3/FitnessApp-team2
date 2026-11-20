using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityService.Features.Login.ViewModels;
using IdentityService.Services.JwtServices;

namespace IdentityService.Features.Login;

public sealed class LoginSessions(
    IGenericRepository<RefreshSession> sessions, UserManager<ApplicationUser> users, ITokenService tokens)
{
    public async Task<LoginSessionViewModel> CreateSessionAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var credentials = tokens.GenerateTokens(user.Id, user.Email!, await users.GetRolesAsync(user));
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
        var validSession = sessions.Get(s => s.TokenHash == hash && s.ExpiresAtUtc > DateTimeOffset.UtcNow
            && s.User.EmailConfirmed && s.User.IsEnabled);
        var session = await validSession.Include(s => s.User).SingleOrDefaultAsync(cancellationToken);
        if (session is null)
            return null;

        var roles = await users.GetRolesAsync(session.User);
        var credentials = tokens.GenerateTokens(session.UserId, session.User.Email!, roles, session.ExpiresAtUtc);
        var newHash = RefreshSession.HashToken(credentials.RefreshToken);
        // Only the request that replaces the old hash may return the new credentials.
        var updated = await validSession.ExecuteUpdateAsync(
            update => update.SetProperty(s => s.TokenHash, newHash), cancellationToken);
        return updated == 1 ? credentials : null;
    }

}
