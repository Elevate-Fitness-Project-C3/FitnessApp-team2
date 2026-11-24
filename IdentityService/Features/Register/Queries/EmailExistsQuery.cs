using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Register.Queries;

public sealed record EmailExistsQuery(string Email) : IRequest<RequestResponse>
{
    public sealed class EmailExistsQueryHandler(UserManager<ApplicationUser> _userManager)
        : IRequestHandler<EmailExistsQuery, RequestResponse>
    {
        public async Task<RequestResponse> Handle(EmailExistsQuery query, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(query.Email.Trim());
            return RequestResponse.Ok("email is founded");
        }
    }
}
