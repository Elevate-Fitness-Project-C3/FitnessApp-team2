using IdentityService.Common.ResultPattern;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Features.Register.Commands;

public  record CreateUserCommand(string name, string email, string password) : IRequest<RequestResponse<CreatedUserDto>>
{

    public sealed class Handler(UserManager<ApplicationUser> _UserManger): IRequestHandler<CreateUserCommand, RequestResponse<CreatedUserDto>>
    {
        public async Task<RequestResponse<CreatedUserDto>> Handle( CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString("N"),
                FullName = command.name.Trim(),
                Email = command.email.Trim(),
                EmailConfirmed = false,
                CreatedAtUtc = DateTime.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString("N"),
                LockoutEnabled = true
            };

            var result = await _UserManger.CreateAsync(user, command.password);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(error => error.Code == "DuplicateEmail"))
                    return RequestResponse<CreatedUserDto>.Fail("This email is already registered.", 409, errorCode: "email_already_registered");

                return RequestResponse<CreatedUserDto>.Fail(string.Join(" ", result.Errors.Select(error => error.Description)),
                    StatusCodes.Status400BadRequest, errorCode: "invalid_user");
            }
            return RequestResponse<CreatedUserDto>.Ok(new CreatedUserDto(user.Id));
        }
    }
}
