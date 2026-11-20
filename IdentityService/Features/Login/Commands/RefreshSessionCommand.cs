using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Context;
using IdentityService.Domain.Entities;
using IdentityService.Features.Login.ViewModels;
using IdentityService.Services.JwtServices;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Login.Commands;

public sealed class RefreshSessionCommand : IRequest<RequestResponse<LoginSessionViewModel>>
{
    public string? RefreshToken { get; init; }

    public sealed class Handler(
        ApplicationDbContext db, UserManager<ApplicationUser> users, ITokenService tokens, TimeProvider clock)
        : IRequestHandler<RefreshSessionCommand, RequestResponse<LoginSessionViewModel>>
    {
        public async Task<RequestResponse<LoginSessionViewModel>> Handle(
            RefreshSessionCommand command, CancellationToken cancellationToken)
        {
            // A 32-byte opaque token encodes to 43 base64url characters.
            if (command.RefreshToken is not { Length: 43 } token)
                return InvalidSession();

            var hash = RefreshSession.HashToken(token);
            var session = await db.RefreshSessions.Include(row => row.User)
                .SingleOrDefaultAsync(row => row.TokenHash == hash, cancellationToken);
            if (session is null || session.ExpiresAtUtc <= clock.GetUtcNow() ||
                !session.User.EmailConfirmed || !session.User.IsEnabled)
                return InvalidSession();

            var roles = await users.GetRolesAsync(session.User);
            var credentials = tokens.GenerateTokens(session.UserId, session.User.Email!, roles, session.ExpiresAtUtc);
            session.TokenHash = RefreshSession.HashToken(credentials.RefreshToken);
            try
            {
                // EF includes the original hash in the update. Only one concurrent request can rotate it.
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return InvalidSession();
            }

            return RequestResponse<LoginSessionViewModel>.Ok(credentials);
        }

        private static RequestResponse<LoginSessionViewModel> InvalidSession() =>
            RequestResponse<LoginSessionViewModel>.Fail(
                "Invalid refresh token.", StatusCodes.Status401Unauthorized, errorCode: "invalid_refresh_token");
    }
}
