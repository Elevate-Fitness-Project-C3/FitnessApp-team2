using IdentityService.Common.ResultPattern;
using MediatR;

namespace IdentityService.Features.VerifyEmail.Commands;

public sealed record VerifyEmailCommand(string? UserId, string? Code) : IRequest<RequestResponse>;
