using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Login.Commands;

public sealed record RotateRefreshTokenCommand(string OldHash, string NewHash) : IRequest<bool>
{
    public sealed class Handler(IGenericRepository<RefreshToken> sessions) : IRequestHandler<RotateRefreshTokenCommand, bool>
    {
        public async Task<bool> Handle(RotateRefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            var updated = await sessions.Get(s => s.TokenHash == command.OldHash && s.ExpiresAtUtc > now && s.User.EmailConfirmed && s.User.IsEnabled)
                .ExecuteUpdateAsync(update => update.SetProperty(s => s.TokenHash, command.NewHash), cancellationToken);
            return updated == 1;
        }
    }
}
