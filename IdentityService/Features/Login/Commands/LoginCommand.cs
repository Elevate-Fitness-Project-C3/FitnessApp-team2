using IdentityService.Common.ResultPattern;
using IdentityService.Features.Login.ViewModels;
using MediatR;

namespace IdentityService.Features.Login.Commands;

public sealed class LoginCommand : IRequest<RequestResponse<LoginSessionViewModel>>
{
    public string? Email { get; init; }
    public string? Password { get; init; }
}
