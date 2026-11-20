using IdentityService.Services.Email;
using MediatR;

namespace IdentityService.Features.Register.Notifications;

public sealed class SendVerificationEmailNotificationHandler(IEmailSender emailSender)
    : INotificationHandler<SendVerificationEmailNotification>
{
    public async Task Handle(SendVerificationEmailNotification notification, CancellationToken cancellationToken)
    {
        var body = $"""
            <p>Your verification code is <strong>{notification.Code}</strong>.</p>
            <p>It expires at {notification.ExpiresAtUtc:yyyy-MM-dd HH:mm:ss} UTC and can be used once.</p>
            <p>If you did not request this account, ignore this email.</p>
            """;

      
    }
}
