using IdentityService.Common.ResultPattern;
using IdentityService.Features.Login.ViewModels;
using IdentityService.Services.JwtServices;
using MediatR;

namespace IdentityService.Features.Login.Commands;

public sealed class RefreshSessionCommand : IRequest<RequestResponse<LoginSessionViewModel>>
{
    public string? RefreshToken { get; init; }

    public sealed class Handler(ITokenService tokens)
        : IRequestHandler<RefreshSessionCommand, RequestResponse<LoginSessionViewModel>>
    {
        public async Task<RequestResponse<LoginSessionViewModel>> Handle(
            RefreshSessionCommand command, CancellationToken cancellationToken)
        {
            var credentials = await tokens.RefreshSessionAsync(command.RefreshToken, cancellationToken);
            return credentials is null
                ? RequestResponse<LoginSessionViewModel>.Fail("Invalid refresh token.",
                    StatusCodes.Status401Unauthorized, errorCode: "invalid_refresh_token")
                : RequestResponse<LoginSessionViewModel>.Ok(credentials);
        }
    }
}
