using IdentityService.Features.Login.DTOs;
using IdentityService.Features.Login.Queries;
using IdentityService.Services.JwtServices;
using MediatR;

namespace IdentityService.Features.Login.Commands;

public sealed record GenerateRefreshCredentialsCommand(RefreshSessionDto Session, List<string> Roles) : IRequest<LoginSessionDto>
{
    public sealed class Handler(ITokenService tokens) : IRequestHandler<GenerateRefreshCredentialsCommand, LoginSessionDto>
    {
        public Task<LoginSessionDto> Handle(GenerateRefreshCredentialsCommand command, CancellationToken cancellationToken)
        {
            var session = command.Session;
            return Task.FromResult(tokens.GenerateTokens(session.UserId, session.Email, command.Roles, session.ExpiresAtUtc));
        }
    }
}
