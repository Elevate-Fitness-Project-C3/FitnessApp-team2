using IdentityService.Common.ResultPattern;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Features.Register.Queries;

public sealed record EmailExistsQuery(string Email) : IRequest<RequestResponse<EmailExistsDto>>
{
    public sealed class EmailExistsQueryHandler(UserManager<ApplicationUser> userManager)
        : IRequestHandler<EmailExistsQuery, RequestResponse<EmailExistsDto>>
    {
        public async Task<RequestResponse<EmailExistsDto>> Handle(EmailExistsQuery query, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(query.Email.Trim());
            return RequestResponse<EmailExistsDto>.Ok(new EmailExistsDto(user is not null));
        }
    }
}
