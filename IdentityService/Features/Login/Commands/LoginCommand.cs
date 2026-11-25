using IdentityService.Common.ResultPattern;
using IdentityService.Features.Login.DTOs;
using MediatR;

namespace IdentityService.Features.Login.Commands;

public sealed class LoginCommand : IRequest<RequestResponse<LoginSessionDto>>
{
    public string? Email { get; init; }
    public string? Password { get; init; }
}
