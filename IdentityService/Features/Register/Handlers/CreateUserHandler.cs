using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.Commands;
using IdentityService.Features.Register.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Features.Register.Handlers;

public sealed class CreateUserHandler(IGenericRepository<ApplicationUser> users)
    : IRequestHandler<CreateUserCommand, RequestResponse<CreatedUserDto>>
{
    private static readonly PasswordHasher<ApplicationUser> Hasher = new();

    public async Task<RequestResponse<CreatedUserDto>> Handle(
        CreateUserCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = Guid.NewGuid().ToString("N"),
            FullName = command.Name.Trim(),
            Email = command.Email.Trim(),
            EmailConfirmed = false,
            CreatedAtUtc = DateTime.UtcNow,
            SecurityStamp = Guid.NewGuid().ToString("N"),
            LockoutEnabled = true
        };

        var normalizer = new UpperInvariantLookupNormalizer();
        user.NormalizedEmail = normalizer.NormalizeEmail(user.Email);
        user.NormalizedUserName = normalizer.NormalizeName(user.UserName);
        user.PasswordHash = Hasher.HashPassword(user, command.Password);
        await users.AddAsync(user, cancellationToken);
        return RequestResponse<CreatedUserDto>.Ok(new CreatedUserDto(user.Id));
    }
}
