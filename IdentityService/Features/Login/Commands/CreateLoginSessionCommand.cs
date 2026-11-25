using IdentityService.DataAccess;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Features.Login.DTOs;
using IdentityService.Services.JwtServices;
using MediatR;

namespace IdentityService.Features.Login.Commands;

public sealed record CreateLoginSessionCommand(AuthenticatedUserDto User) : IRequest<LoginSessionDto>
{
    public sealed class Handler(IGenericRepository<RefreshToken> sessions, ITokenService tokens, UnitOfWork unitOfWork)
        : IRequestHandler<CreateLoginSessionCommand, LoginSessionDto>
    {
        public async Task<LoginSessionDto> Handle(CreateLoginSessionCommand command, CancellationToken cancellationToken)
        {
            var user = command.User;
            var session = tokens.GenerateTokens(user.UserId, user.Email, user.Roles);
            await sessions.AddAsync(new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = RefreshToken.HashToken(session.RefreshToken),
                ExpiresAtUtc = session.RefreshTokenExpiresAtUtc
            }, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return session;
        }
    }
}
