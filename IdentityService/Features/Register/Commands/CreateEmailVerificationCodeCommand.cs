using IdentityService.Common.ResultPattern;
using IdentityService.Features.Register.DTOs;
using MediatR;

namespace IdentityService.Features.Register.Commands;

public sealed record CreateEmailVerificationCodeCommand(string UserId) : IRequest<RequestResponse<CreatedVerificationCodeDto>>;
