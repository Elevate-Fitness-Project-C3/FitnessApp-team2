using IdentityService.Common.ResultPattern;
using IdentityService.Features.Login.Commands;
using IdentityService.Features.Login.DTOs;
using MediatR;

namespace IdentityService.Features.Login.Orchestrators;

public sealed class LoginOrchestrator(IMediator mediator) : IRequestHandler<LoginCommand, RequestResponse<LoginSessionDto>>
{
    public async Task<RequestResponse<LoginSessionDto>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email?.Trim();
        if (string.IsNullOrEmpty(email) || email.Length > 256 || string.IsNullOrEmpty(command.Password))
            return RequestResponse<LoginSessionDto>.Fail("Enter your email and password.", 400, errorCode: "invalid_request");

        var user = await mediator.Send(new CheckLoginCredentialsCommand(email, command.Password), cancellationToken);
        if (user is null)
            return RequestResponse<LoginSessionDto>.Fail("Invalid email or password.", 401, errorCode: "invalid_credentials");

        var session = await mediator.Send(new CreateLoginSessionCommand(user), cancellationToken);
        return RequestResponse<LoginSessionDto>.Ok(session);
    }
}
