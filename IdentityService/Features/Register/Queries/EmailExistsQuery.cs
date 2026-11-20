using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Register.Queries;

public sealed record EmailExistsQuery(string Email) : IRequest<RequestResponse<EmailExistsDto>>
{
    public sealed class Handler(IGenericRepository<ApplicationUser> users)
        : IRequestHandler<EmailExistsQuery, RequestResponse<EmailExistsDto>>
    {
        public async Task<RequestResponse<EmailExistsDto>> Handle(EmailExistsQuery query, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var normalizedEmail = new UpperInvariantLookupNormalizer().NormalizeEmail(query.Email.Trim());
            var exists = await users.Get(x => x.NormalizedEmail == normalizedEmail).AnyAsync(cancellationToken);
            return RequestResponse<EmailExistsDto>.Ok(new EmailExistsDto(exists));
        }
    }
}
