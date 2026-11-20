using IdentityService.Common.ResultPattern;
using IdentityService.Features.Login.ViewModels;
using IdentityService.Services.JwtServices;
using MediatR;

namespace IdentityService.Features.Login.Commands;

public sealed record CreateLoginSessionCommand(string UserId) : IRequest<RequestResponse<LoginSessionViewModel>>
{
    public sealed class Handler(ITokenService tokens)
        : IRequestHandler<CreateLoginSessionCommand, RequestResponse<LoginSessionViewModel>>
    {
        public async Task<RequestResponse<LoginSessionViewModel>> Handle(
            CreateLoginSessionCommand command, CancellationToken cancellationToken)
        {
            var credentials = await tokens.CreateSessionAsync(command.UserId, cancellationToken);
            return credentials is null
                ? RequestResponse<LoginSessionViewModel>.Fail("Invalid email or password.",
                    StatusCodes.Status401Unauthorized, errorCode: "invalid_credentials")
                : RequestResponse<LoginSessionViewModel>.Ok(credentials);
        }
    }
}
