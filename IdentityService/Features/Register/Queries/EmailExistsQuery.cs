using IdentityService.Common.ResultPattern;
using IdentityService.Features.Register.DTOs;
using MediatR;

namespace IdentityService.Features.Register.Queries;

public sealed record EmailExistsQuery(string Email) : IRequest<RequestResponse<EmailExistsDto>>;
