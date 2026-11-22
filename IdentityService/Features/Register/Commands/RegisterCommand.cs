using IdentityService.Common.ResultPattern;
using IdentityService.Features.Register.ViewModels;
using MediatR;

namespace IdentityService.Features.Register.Commands;

public sealed class RegisterCommand : IRequest<RequestResponse<RegisterViewModel>>
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
}
