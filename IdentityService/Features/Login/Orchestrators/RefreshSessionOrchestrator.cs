using IdentityService.Common.ResultPattern;
using IdentityService.Domain.Entities;
using IdentityService.Features.Login.Commands;
using IdentityService.Features.Login.DTOs;
using IdentityService.Features.Login.Queries;
using MediatR;

namespace IdentityService.Features.Login.Orchestrators;

public sealed class RefreshSessionOrchestrator(IMediator mediator) : IRequestHandler<RefreshSessionCommand, RequestResponse<LoginSessionDto>>
{
    public async Task<RequestResponse<LoginSessionDto>> Handle(RefreshSessionCommand command, CancellationToken cancellationToken)
    {
        var invalidToken = RequestResponse<LoginSessionDto>.Fail("Invalid refresh token.", 401, errorCode: "invalid_refresh_token");
        if (command.RefreshToken is not { Length: 43 })
            return invalidToken;

        var oldHash = RefreshToken.HashToken(command.RefreshToken);
        var session = await mediator.Send(new GetRefreshSessionQuery(oldHash), cancellationToken);
        if (session is null)
            return invalidToken;

        var roleIds = await mediator.Send(new GetUserRoleIdsQuery(session.UserId), cancellationToken);
        var roles = await mediator.Send(new GetRoleNamesQuery(roleIds), cancellationToken);
        var credentials = await mediator.Send(new GenerateCredentialsQuery(session.UserId, session.Email, roles, session.ExpiresAtUtc), cancellationToken);
        var newHash = RefreshToken.HashToken(credentials.RefreshToken);
        var rotated = await mediator.Send(new RotateRefreshTokenCommand(oldHash, newHash), cancellationToken);
        return rotated ? RequestResponse<LoginSessionDto>.Ok(credentials) : invalidToken;
    }
}
