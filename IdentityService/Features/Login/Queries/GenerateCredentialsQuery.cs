using IdentityService.Features.Login.DTOs;
using IdentityService.Services.JwtServices;
using MediatR;

namespace IdentityService.Features.Login.Queries;

public sealed record GenerateCredentialsQuery(string UserId, string Email, IEnumerable<string> Roles,
    DateTimeOffset? RefreshExpiresAtUtc = null) : IRequest<LoginSessionDto>
{
    public sealed class Handler(ITokenService tokens) : IRequestHandler<GenerateCredentialsQuery, LoginSessionDto>
    {
        public Task<LoginSessionDto> Handle(GenerateCredentialsQuery query, CancellationToken cancellationToken) =>
            Task.FromResult(tokens.GenerateTokens(query.UserId, query.Email, query.Roles, query.RefreshExpiresAtUtc));
    }
}
