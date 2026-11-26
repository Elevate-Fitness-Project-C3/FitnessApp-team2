using IdentityService.DataAccess.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Login.Queries;

public sealed record GetRoleNamesQuery(List<string> RoleIds) : IRequest<List<string>>
{
    public sealed class Handler(IGenericRepository<IdentityRole> roles) : IRequestHandler<GetRoleNamesQuery, List<string>>
    {
        public Task<List<string>> Handle(GetRoleNamesQuery query, CancellationToken cancellationToken) =>
            roles.Get(r => query.RoleIds.Contains(r.Id)).Select(r => r.Name!).ToListAsync(cancellationToken);
    }
}
