using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain;
using IdentityService.Features.Register.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Register.Commands;

public sealed record AssignUserRoleCommand(string UserId) : IRequest<RequestResponse<AssignedUserRoleDto>>
{
    public sealed class Handler(
        IGenericRepository<IdentityRole> roles,
        IGenericRepository<IdentityUserRole<string>> userRoles)
        : IRequestHandler<AssignUserRoleCommand, RequestResponse<AssignedUserRoleDto>>
    {
        public async Task<RequestResponse<AssignedUserRoleDto>> Handle(
            AssignUserRoleCommand command, CancellationToken cancellationToken)
        {
            var normalizedName = new UpperInvariantLookupNormalizer().NormalizeName(RoleNames.User);
            var role = await roles.Get(x => x.NormalizedName == normalizedName)
                .SingleOrDefaultAsync(cancellationToken);
            if (role is null)
                return RequestResponse<AssignedUserRoleDto>.Fail(
                    "Registration is temporarily unavailable. The User role must be initialized.",
                    StatusCodes.Status503ServiceUnavailable, errorCode: "registration_role_missing");

            await userRoles.AddAsync(
                new IdentityUserRole<string> { UserId = command.UserId, RoleId = role.Id },
                cancellationToken);

            return RequestResponse<AssignedUserRoleDto>.Ok(
                new AssignedUserRoleDto(command.UserId, role.Id));
        }
    }
}
