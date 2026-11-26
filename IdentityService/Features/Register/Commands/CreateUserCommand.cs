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
                FullName = command.name.Trim(),
                UserName = command.email,
                Email = command.email.Trim(),
                EmailConfirmed = false,
                CreatedAtUtc = DateTime.UtcNow,
                LockoutEnabled = true
            };

            var result = await _UserManger.CreateAsync(user, command.password);
            if (!result.Succeeded)
            {
                return RequestResponse<CreatedUserDto>.Fail(string.Join(" ", result.Errors.Select(error => error.Description)),
                    StatusCodes.Status400BadRequest, errorCode: "invalid_user");
            }
            return RequestResponse<CreatedUserDto>.Ok(new CreatedUserDto(user.Id));
        }
    }
}
