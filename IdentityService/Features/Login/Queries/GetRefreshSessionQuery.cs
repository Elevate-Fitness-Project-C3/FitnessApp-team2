using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Login.Queries;

public sealed record RefreshSessionDto(string UserId, string Email, DateTimeOffset ExpiresAtUtc);

public sealed record GetRefreshSessionQuery(string TokenHash) : IRequest<RefreshSessionDto?>
{
    public sealed class Handler(IGenericRepository<RefreshToken> sessions) : IRequestHandler<GetRefreshSessionQuery, RefreshSessionDto?>
    {
        public Task<RefreshSessionDto?> Handle(GetRefreshSessionQuery query, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            return sessions.Get(s => s.TokenHash == query.TokenHash && s.ExpiresAtUtc > now && s.User.EmailConfirmed && s.User.IsEnabled)
                .Select(s => new RefreshSessionDto(s.UserId, s.User.Email!, s.ExpiresAtUtc))
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
