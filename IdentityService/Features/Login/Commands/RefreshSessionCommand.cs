using IdentityService.Common.ResultPattern;
using IdentityService.Features.Login.DTOs;
using MediatR;

namespace IdentityService.Features.Login.Commands;

public sealed class RefreshSessionCommand : IRequest<RequestResponse<LoginSessionDto>>
{
    public string? RefreshToken { get; init; }
}
