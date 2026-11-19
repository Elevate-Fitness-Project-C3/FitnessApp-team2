using IdentityService.Common.ResultPattern;
using IdentityService.Features.Register.DTOs;
using MediatR;

namespace IdentityService.Features.Register.Commands;

public sealed class CreateUserCommand(string name, string email, string password) : IRequest<RequestResponse<CreatedUserDto>>
{
    public string Name { get; } = name;
    public string Email { get; } = email;
    public string Password { get; } = password;
}
