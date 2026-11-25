using IdentityService.Common.ResultPattern;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Register.Commands;

public record CreateUserCommand(string name, string email, string password) : IRequest<RequestResponse<CreatedUserDto>>
{
    public sealed class Handler(UserManager<ApplicationUser> userManager)
        : IRequestHandler<CreateUserCommand, RequestResponse<CreatedUserDto>>
    {
        public async Task<RequestResponse<CreatedUserDto>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var email = command.email.Trim();
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                FullName = command.name.Trim(),
                UserName = email,
                Email = email,
                EmailConfirmed = false,
                CreatedAtUtc = DateTime.UtcNow,
                LockoutEnabled = true
            };

            IdentityResult result;
            try
            {
                result = await userManager.CreateAsync(user, command.password);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                // Another registration may have committed after the availability check.
                if (await userManager.FindByEmailAsync(email) is null)
                    throw;

                return RegistrationErrors.EmailAlreadyRegistered<CreatedUserDto>();
            }

            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code is nameof(IdentityErrorDescriber.DuplicateEmail)
                    or nameof(IdentityErrorDescriber.DuplicateUserName)))
                    return RegistrationErrors.EmailAlreadyRegistered<CreatedUserDto>();

                return RequestResponse<CreatedUserDto>.Fail(string.Join(" ", result.Errors.Select(error => error.Description)),
                    StatusCodes.Status400BadRequest, errorCode: "invalid_user");
            }
            return RequestResponse<CreatedUserDto>.Ok(new CreatedUserDto(user.Id));
        }
    }
}
