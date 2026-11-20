using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Context;
using IdentityService.Domain.Entities;
using IdentityService.Features.Login.ViewModels;
using IdentityService.Services.JwtServices;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Features.Login.Commands;

public sealed record CreateLoginSessionCommand(string UserId) : IRequest<RequestResponse<LoginSessionViewModel>>
{
    public sealed class Handler(
        UserManager<ApplicationUser> users, ITokenService tokens, ApplicationDbContext db)
        : IRequestHandler<CreateLoginSessionCommand, RequestResponse<LoginSessionViewModel>>
    {
        public async Task<RequestResponse<LoginSessionViewModel>> Handle(
            CreateLoginSessionCommand command, CancellationToken cancellationToken)
        {
            var user = await users.FindByIdAsync(command.UserId);
            if (user is null || !user.IsEnabled || !user.EmailConfirmed)
                return RequestResponse<LoginSessionViewModel>.Fail(
                    "Invalid email or password.", StatusCodes.Status401Unauthorized, errorCode: "invalid_credentials");

            var credentials = tokens.GenerateTokens(user.Id, user.Email!, await users.GetRolesAsync(user));
            db.RefreshSessions.Add(new RefreshSession
            {
                UserId = user.Id,
                TokenHash = RefreshSession.HashToken(credentials.RefreshToken),
                ExpiresAtUtc = credentials.RefreshTokenExpiresAtUtc
            });
            await db.SaveChangesAsync(cancellationToken);
            return RequestResponse<LoginSessionViewModel>.Ok(credentials);
        }
    }
}
