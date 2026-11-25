using IdentityService.DataAccess.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Login.Queries;

public sealed record GetUserRoleIdsQuery(string UserId) : IRequest<List<string>>
{
    public sealed class Handler(IGenericRepository<IdentityUserRole<string>> memberships) : IRequestHandler<GetUserRoleIdsQuery, List<string>>
    {
        public Task<List<string>> Handle(GetUserRoleIdsQuery query, CancellationToken cancellationToken) =>
            memberships.Get(m => m.UserId == query.UserId).Select(m => m.RoleId).ToListAsync(cancellationToken);
    }
}
