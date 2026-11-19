using IdentityService.Common.ResultPattern;
using IdentityService.Features.Register.Commands;
using IdentityService.Features.Register.Exceptions;
using IdentityService.Features.Register.Validators;
using IdentityService.Features.Register.ViewModels;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.Register.Behaviors;

public sealed class RegisterBehavior : IPipelineBehavior<RegisterCommand, RequestResponse<RegisterViewModel>>
{
    public async Task<RequestResponse<RegisterViewModel>> Handle(
        RegisterCommand request, CancellationToken cancellationToken,
        RequestHandlerDelegate<RequestResponse<RegisterViewModel>> next)
    {
        var errors = RegisterValidator.Validate(request);
        if (errors.Count > 0)
            return RequestResponse<RegisterViewModel>.Fail(
                "Correct the highlighted fields.", StatusCodes.Status400BadRequest, errors, "validation_failed");

        try
        {
            return await next();
        }
        catch (RegistrationException exception)
        {
            return RequestResponse<RegisterViewModel>.Fail(
                exception.Message, exception.StatusCode, errorCode: exception.ErrorCode);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 } sql &&
            sql.Message.Contains("EmailIndex", StringComparison.Ordinal))
        {
            return RequestResponse<RegisterViewModel>.Fail(
                "This email is already registered. Sign in with your existing account or use another email.",
                StatusCodes.Status409Conflict, errorCode: "email_already_registered");
        }
    }
}
