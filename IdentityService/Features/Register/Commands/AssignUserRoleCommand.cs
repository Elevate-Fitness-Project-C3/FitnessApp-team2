using IdentityService.Common.ResultPattern;
using IdentityService.Domain;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static MassTransit.ValidationResultExtensions;

namespace IdentityService.Features.Register.Commands;

public sealed record AssignUserRoleCommand(string UserId) : IRequest<RequestResponse<AssignedUserRoleDto>>
{
    public sealed class AssignUserRoleCommandHandler :
        IRequestHandler<AssignUserRoleCommand, RequestResponse<AssignedUserRoleDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AssignUserRoleCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<RequestResponse<AssignedUserRoleDto>> Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if(user is null)
            {
                return RequestResponse<AssignedUserRoleDto>.Fail(
                  "User was not found.",
                  StatusCodes.Status404NotFound,
                  errorCode: "user_not_found");
            }

            var role =await _userManager.AddToRoleAsync(user,RoleNames.User);
            if (!role.Succeeded)
            {
                var errors = new Dictionary<string, string[]>
                {
                    ["errors"] = role.Errors.Select(error => error.Description).ToArray()
                };
                return RequestResponse<AssignedUserRoleDto>.Fail(
                    "Could not assign the default role.",
                    StatusCodes.Status500InternalServerError,
                    errors: errors,
                    errorCode: "role_assignment_failed");
            }
            return RequestResponse<AssignedUserRoleDto>.Ok(
                new AssignedUserRoleDto(user.Id));
        }
    }
}
