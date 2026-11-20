using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Features.Login.Commands;

public sealed class CheckLoginCredentialsCommand(string email, string password) : IRequest<string?>
{
    public string Email { get; } = email;
    public string Password { get; } = password;

    public sealed class Handler(SignInManager<ApplicationUser> signIn) : IRequestHandler<CheckLoginCredentialsCommand, string?>
    {
        private static readonly ApplicationUser DummyUser = new() { FullName = string.Empty };
        private static readonly string DummyHash =
            new PasswordHasher<ApplicationUser>().HashPassword(DummyUser, Guid.NewGuid().ToString());

        public async Task<string?> Handle(CheckLoginCredentialsCommand request, CancellationToken cancellationToken)
        {
            var users = signIn.UserManager;
            var user = await users.FindByEmailAsync(request.Email);
            if (user is null || !user.IsEnabled || !user.EmailConfirmed || await users.IsLockedOutAsync(user))
            {
                users.PasswordHasher.VerifyHashedPassword(DummyUser, DummyHash, request.Password);
                return null;
            }

            if (!user.LockoutEnabled && !(await users.SetLockoutEnabledAsync(user, true)).Succeeded)
                return null;

            var result = await signIn.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            return result.Succeeded ? user.Id : null;
        }
    }
}
