using IdentityService.Common.ResultPattern;
using IdentityService.Features.VerifyEmail.Commands;
using MediatR;

namespace IdentityService.Features.VerifyEmail.Orchestrators;

public sealed class VerifyEmailOrchestrator(IMediator mediator) : IRequestHandler<VerifyEmailCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        var invalidCode = RequestResponse.Fail("Invalid or expired verification code.", errorCode: "invalid_verification_code");
        if (string.IsNullOrEmpty(command.UserId) || command.Code is not { Length: 6 } || !command.Code.All(char.IsAsciiDigit))
            return invalidCode;

        var verified = await mediator.Send(new ConfirmUserEmailCommand(command.UserId, command.Code), cancellationToken);
        return verified ? RequestResponse.Ok("Email verified.") : invalidCode;
    }
}
