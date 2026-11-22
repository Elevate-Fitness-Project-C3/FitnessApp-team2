using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Features.Login.Commands;

public sealed class CheckLoginCredentialsCommand(string email, string password) : IRequest<ApplicationUser?>
{
    public string Email { get; } = email;
    public string Password { get; } = password;

    public sealed class Handler(SignInManager<ApplicationUser> signIn) : IRequestHandler<CheckLoginCredentialsCommand, ApplicationUser?>
    {
        private static readonly ApplicationUser DummyUser = new() { FullName = string.Empty };
        private static readonly string DummyHash =
            new PasswordHasher<ApplicationUser>().HashPassword(DummyUser, Guid.NewGuid().ToString());

        public async Task<ApplicationUser?> Handle(CheckLoginCredentialsCommand request, CancellationToken cancellationToken)
        {
            var users = signIn.UserManager;
            // Identity's SQL Server store exposes the user set. Serialize password attempts
            // until the orchestrator commits the counter or the successful session.
            var userSet = (DbSet<ApplicationUser>)users.Users;
            var normalized = users.NormalizeEmail(request.Email);
            var user = await userSet.FromSqlInterpolated(
                $"SELECT * FROM dbo.AspNetUsers WITH (UPDLOCK) WHERE NormalizedEmail = {normalized}")
                .SingleOrDefaultAsync(cancellationToken);
            if (user is null || !user.IsEnabled || !user.EmailConfirmed || await users.IsLockedOutAsync(user))
            {
                users.PasswordHasher.VerifyHashedPassword(DummyUser, DummyHash, request.Password);
                return null;
            }

            if (!user.LockoutEnabled && !(await users.SetLockoutEnabledAsync(user, true)).Succeeded)
                return null;

            var result = await signIn.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            return result.Succeeded ? user : null;
        }
    }
}
