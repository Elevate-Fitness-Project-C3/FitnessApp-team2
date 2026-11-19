using System.Globalization;
using System.Security.Cryptography;
using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.Commands;
using IdentityService.Features.Register.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Features.Register.Handlers;

public sealed class CreateEmailVerificationCodeHandler(IGenericRepository<EmailVerificationCode> codes)
    : IRequestHandler<CreateEmailVerificationCodeCommand, RequestResponse<CreatedVerificationCodeDto>>
{
    private static readonly PasswordHasher<EmailVerificationCode> Hasher = new();

    public async Task<RequestResponse<CreatedVerificationCodeDto>> Handle(
        CreateEmailVerificationCodeCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var plainCode = RandomNumberGenerator.GetInt32(1_000_000).ToString("D6", CultureInfo.InvariantCulture);
        var now = DateTime.UtcNow;
        var verification = new EmailVerificationCode
        {
            UserId = command.UserId,
            CodeHash = "",
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(10)
        };
        verification.CodeHash = Hasher.HashPassword(verification, plainCode);
        await codes.AddAsync(verification, cancellationToken);

        return RequestResponse<CreatedVerificationCodeDto>.Ok(
            new CreatedVerificationCodeDto { Code = plainCode, ExpiresAtUtc = verification.ExpiresAtUtc });
    }
}
