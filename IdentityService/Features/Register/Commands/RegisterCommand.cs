using IdentityService.Common.ResultPattern;
using IdentityService.Features.Register.ViewModels;
using MediatR;

namespace IdentityService.Features.Register.Commands;

public sealed class RegisterCommand : IRequest<RequestResponse<RegisterViewModel>>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
