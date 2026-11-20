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

        // A denied login must still commit Identity's failed-attempt count and any new lockout.
        await unitOfWork.ExecuteLoginAsync(email, async () =>
        {
            var userId = await mediator.Send(new CheckLoginCredentialsCommand(email, command.Password), cancellationToken);
            if (userId is null)
                return;

            response = await mediator.Send(new CreateLoginSessionCommand(userId), cancellationToken);
            if (response.Success)
                await unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return response;
    }
}
