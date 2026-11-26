using IdentityService.DataAccess;
using IdentityService.DataAccess.Context;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using MediatR;

namespace IdentityService.Features.Login.Commands;

public sealed record CreateLoginSessionCommand(string UserId, string TokenHash, DateTimeOffset ExpiresAtUtc) : IRequest<Guid>
{
    public sealed class Handler(IGenericRepository<RefreshToken> _refreshtoken,UnitOfWork _unitOfWork) : IRequestHandler<CreateLoginSessionCommand, Guid>
    {
        public async Task<Guid> Handle(CreateLoginSessionCommand command, CancellationToken cancellationToken)
        {
            var session = new RefreshToken
            {
                UserId = command.UserId,
                TokenHash = command.TokenHash,
                ExpiresAtUtc = command.ExpiresAtUtc
            };
            await _refreshtoken.AddAsync(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return session.Id;
        }
    }
}
