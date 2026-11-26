using IdentityService.DataAccess;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.VerifyEmail.Commands;

public sealed record ConfirmUserEmailCommand(string UserId, string Code) : IRequest<bool>
{
    public sealed class Handler(IGenericRepository<EmailVerificationCode> codes, UnitOfWork unitOfWork)
        : IRequestHandler<ConfirmUserEmailCommand, bool>
    {
        private const int MaximumAttempts = 5;
        private static readonly PasswordHasher<EmailVerificationCode> Hasher = new();

        public async Task<bool> Handle(ConfirmUserEmailCommand command, CancellationToken cancellationToken)
        {
            var confirmed = false;
            await unitOfWork.ExecuteAsync(async () =>
            {
                var now = DateTime.UtcNow;
                var userCode = codes.Get(code => code.UserId == command.UserId, tracking: true);
                var attemptsUpdated = await userCode
                    .Where(code => code.ExpiresAtUtc > now && code.AttemptCount < MaximumAttempts)
                    .ExecuteUpdateAsync(update => update.SetProperty(code => code.AttemptCount, code => code.AttemptCount + 1),
                        cancellationToken);
                if (attemptsUpdated == 0)
                    return;

                var code = await userCode.Include(code => code.User).SingleAsync(cancellationToken);
                if (Hasher.VerifyHashedPassword(code, code.CodeHash, command.Code) == PasswordVerificationResult.Failed)
                    return;

                code.User.EmailConfirmed = true;
                codes.Delete(code);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                confirmed = true;
            }, cancellationToken);
            return confirmed;
        }
    }
}
