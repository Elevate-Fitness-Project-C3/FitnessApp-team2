using IdentityService.Common.ResultPattern;
using IdentityService.DataAccess;
using IdentityService.Features.Register.Commands;
using IdentityService.Features.Register.Notifications;
using IdentityService.Features.Register.Queries;
using IdentityService.Features.Register.ViewModels;
using Gym.Contracts;
using MassTransit;
using MediatR;

namespace IdentityService.Features.Register.Orchestrators;

public sealed class RegisterOrchestrator(IMediator mediator, UnitOfWork unitOfWork, IPublishEndpoint publisher)
    : IRequestHandler<RegisterCommand, RequestResponse<RegisterViewModel>>
{
    public async Task<RequestResponse<RegisterViewModel>> Handle(
        RegisterCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email!.Trim();
        var existing = await mediator.Send(new EmailExistsQuery(email), cancellationToken);
        if (existing.Data!.Exists)
            return RequestResponse<RegisterViewModel>.Fail(
                "This email is already registered. Sign in with your existing account or use another email.",
                StatusCodes.Status409Conflict, errorCode: "email_already_registered");

        RegisterViewModel registered = null!;

        await unitOfWork.ExecuteAsync(async () =>
        {
            var user = await mediator.Send(
                new CreateUserCommand(command.Name!, email, command.Password!), cancellationToken);
            var userId = user.Data!.UserId;

            await mediator.Send(new AssignUserRoleCommand(userId), cancellationToken);

            var verification = await mediator.Send(
                new CreateEmailVerificationCodeCommand(userId), cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new SendVerificationEmailNotification(
                email, verification.Data!.Code, verification.Data.ExpiresAtUtc), cancellationToken);

            await publisher.Publish(new UserRegistered(userId), cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            registered = new RegisterViewModel(userId, email, false, "pending");
        }, cancellationToken);

        return RequestResponse<RegisterViewModel>.Ok(
            registered, "Your account was created. Check your email for the verification code.",
            StatusCodes.Status201Created);
    }
}
