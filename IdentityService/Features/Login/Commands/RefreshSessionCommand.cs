using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Features.Login.DTOs;
using IdentityService.Services.JwtServices;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoredRefreshToken = IdentityService.Domain.Entities.RefreshToken;

namespace IdentityService.Features.Login.Commands;

public sealed class RefreshSessionCommand : IRequest<RequestResponse<LoginSessionDto>>
{
    public string? RefreshToken { get; init; }

    public sealed class Handler(
        IGenericRepository<StoredRefreshToken> sessions,
        UserManager<ApplicationUser> users,
        ITokenService tokens)
        : IRequestHandler<RefreshSessionCommand, RequestResponse<LoginSessionDto>>
    {
        public async Task<RequestResponse<LoginSessionDto>> Handle(
            RefreshSessionCommand command, CancellationToken cancellationToken)
        {
            var invalidToken = RequestResponse<LoginSessionDto>.Fail("Invalid refresh token.", 401, errorCode: "invalid_refresh_token");
            if (command.RefreshToken is not { Length: 43 })
                return invalidToken;

            var hash = StoredRefreshToken.HashToken(command.RefreshToken);
            var now = DateTimeOffset.UtcNow;
            var validSession = sessions.Get(s => s.TokenHash == hash && s.ExpiresAtUtc > now
                && s.User.EmailConfirmed && s.User.IsEnabled);
            var session = await validSession
                .Select(s => new { s.User, s.ExpiresAtUtc })
                .SingleOrDefaultAsync(cancellationToken);
            if (session is null)
                return invalidToken;

            var roles = await users.GetRolesAsync(session.User);
            var credentials = tokens.GenerateTokens(session.User.Id, session.User.Email!, roles, session.ExpiresAtUtc);
            var newHash = StoredRefreshToken.HashToken(credentials.RefreshToken);
            now = DateTimeOffset.UtcNow;
            var updated = await validSession.ExecuteUpdateAsync(
                update => update.SetProperty(s => s.TokenHash, newHash), cancellationToken);
            return updated == 1 ? RequestResponse<LoginSessionDto>.Ok(credentials) : invalidToken;
        }
    }
}
