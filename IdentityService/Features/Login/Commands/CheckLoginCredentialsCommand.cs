using IdentityService.Domain.Entities;
using IdentityService.Features.Login.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Features.Login.Commands;

public sealed record CheckLoginCredentialsCommand(string Email, string Password) : IRequest<AuthenticatedUserDto?>
{
    public sealed class Handler(SignInManager<ApplicationUser> signIn)
        : IRequestHandler<CheckLoginCredentialsCommand, AuthenticatedUserDto?>
    {
        public async Task<AuthenticatedUserDto?> Handle(CheckLoginCredentialsCommand command, CancellationToken cancellationToken)
        {
            var users = signIn.UserManager;
            var user = await users.FindByEmailAsync(command.Email);
            if (user is null || !user.IsEnabled || !user.EmailConfirmed || await users.IsLockedOutAsync(user))
            {
                users.PasswordHasher.HashPassword(user!, command.Password);
                return null;
            }

            if (!user.LockoutEnabled && !(await users.SetLockoutEnabledAsync(user, true)).Succeeded)
                return null;

            var check = await signIn.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: true);
            if (!check.Succeeded)
                return null;

            return new AuthenticatedUserDto(user.Id, user.Email!, await users.GetRolesAsync(user));
        }
    }
}
