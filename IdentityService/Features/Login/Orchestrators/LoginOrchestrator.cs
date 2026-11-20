using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess;
using IdentityService.Features.Login.Commands;
using IdentityService.Features.Login.ViewModels;
using MediatR;

namespace IdentityService.Features.Login.Orchestrators;

public sealed class LoginOrchestrator(IMediator mediator, UnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, RequestResponse<LoginSessionViewModel>>
{
    public async Task<RequestResponse<LoginSessionViewModel>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email?.Trim();
        if (string.IsNullOrEmpty(email) || email.Length > 256 || string.IsNullOrEmpty(command.Password))
            return RequestResponse<LoginSessionViewModel>.Fail(
                "Enter your email and password.", StatusCodes.Status400BadRequest, errorCode: "invalid_request");

        var response = RequestResponse<LoginSessionViewModel>.Fail(
            "Invalid email or password.", StatusCodes.Status401Unauthorized, errorCode: "invalid_credentials");

        await unitOfWork.ExecuteAsync(async () =>
        {
            var user = await mediator.Send(new CheckLoginCredentialsCommand(email, command.Password), cancellationToken);
            if (user is null)
                return;

            var session = await mediator.Send(new CreateLoginSessionCommand(user), cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            response = RequestResponse<LoginSessionViewModel>.Ok(session);
        }, cancellationToken);

        return response;
    }
}
