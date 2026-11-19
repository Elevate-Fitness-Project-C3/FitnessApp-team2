using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain;
using IdentityService.Features.Register.Commands;
using IdentityService.Features.Register.DTOs;
using IdentityService.Features.Register.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Register.Handlers;

public sealed class AssignUserRoleHandler(
    IGenericRepository<IdentityRole> roles,
    IGenericRepository<IdentityUserRole<string>> userRoles)
    : IRequestHandler<AssignUserRoleCommand, RequestResponse<AssignedUserRoleDto>>
{
    public async Task<RequestResponse<AssignedUserRoleDto>> Handle(
        AssignUserRoleCommand command, CancellationToken cancellationToken)
    {
        var normalizedName = new UpperInvariantLookupNormalizer().NormalizeName(RoleNames.User);
        var role = await roles.Get(x => x.NormalizedName == normalizedName)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new RegistrationException(
                "Registration is temporarily unavailable. The User role must be initialized.",
                StatusCodes.Status503ServiceUnavailable, "registration_role_missing");

        await userRoles.AddAsync(
            new IdentityUserRole<string> { UserId = command.UserId, RoleId = role.Id },
            cancellationToken);

        return RequestResponse<AssignedUserRoleDto>.Ok(
            new AssignedUserRoleDto(command.UserId, role.Id));
    }
}
