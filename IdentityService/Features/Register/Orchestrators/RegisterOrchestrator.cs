using Gym.Contracts;
using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess;
using IdentityService.Features.Register.Commands;
using IdentityService.Features.Register.Notifications;
using IdentityService.Features.Register.Queries;
using IdentityService.Features.Register.Validators;
using IdentityService.Features.Register.ViewModels;
using MassTransit;
using MediatR;

namespace IdentityService.Features.Register.Orchestrators;

public sealed class RegisterOrchestrator
    : IRequestHandler<RegisterCommand, RequestResponse<RegisterViewModel>>
{
    private readonly IMediator _mediator;
    private readonly UnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publisher;

    public RegisterOrchestrator(IMediator mediator, UnitOfWork unitOfWork, IPublishEndpoint publisher)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task<RequestResponse<RegisterViewModel>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var errors = RegisterValidator.Validate(command);
        if (errors.Count > 0)
            return RequestResponse<RegisterViewModel>.Fail("Invalid registration details.", errors: errors);

        var email = command.Email.Trim();
        string userId = null!;
        SendVerificationEmailNotification verificationEmail = null!;
        var lookup = await _mediator.Send(new EmailExistsQuery(email), cancellationToken);

        if (!lookup.Success)
        {
            return RequestResponse<RegisterViewModel>.Fail(
                lookup.Message, lookup.StatusCode, lookup.Errors, lookup.ErrorCode);
        }


        try
        {
            await _unitOfWork.ExecuteAsync(async () =>
            {
                var user = await _mediator.Send(new CreateUserCommand(command.Name, email, command.Password), cancellationToken);

                if (!user.Success)
                {
                    throw new RegistrationFailedException(user.Message, user.StatusCode, user.Errors, user.ErrorCode);
                }

                userId = user.Data!.UserId;

                var role = await _mediator.Send(new AssignUserRoleCommand(userId), cancellationToken);

                if (!role.Success)
                {
                    throw new RegistrationFailedException(role.Message, role.StatusCode, role.Errors, role.ErrorCode);
                }

                var otp = await _mediator.Send(new CreateEmailVerificationCodeCommand(userId), cancellationToken);

                if (!otp.Success)
                {
                    throw new RegistrationFailedException(otp.Message, otp.StatusCode, otp.Errors, otp.ErrorCode);
                }

                var code = otp.Data!;
                verificationEmail = new SendVerificationEmailNotification(email, code.Code, code.ExpiresAtUtc);

                await _publisher.Publish(new UserRegistered(userId), cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);
        }
        catch (RegistrationFailedException ex)
        {
            return ex.Result;
        }

        try
        {
            await _mediator.Publish(verificationEmail);
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            return RequestResponse<RegisterViewModel>.Fail(
                "The account was created, but the verification email could not be sent.",
                StatusCodes.Status503ServiceUnavailable, errorCode: "verification_email_failed");
        }

        return RequestResponse<RegisterViewModel>.Ok(new RegisterViewModel(userId, email, false, "pending"),
            "Your account was created.",
            StatusCodes.Status201Created);
    }

    private sealed class RegistrationFailedException(
        string message,
        int statusCode,
        IDictionary<string, string[]>? errors,
        string? errorCode) : Exception(message)
    {
        public RequestResponse<RegisterViewModel> Result { get; } = RequestResponse<RegisterViewModel>.Fail(message, statusCode, errors, errorCode);
    }

}
