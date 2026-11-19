using IdentityService.Messaging;
using MediatR;

namespace IdentityService.Features.Register.Notifications;

public sealed class UserRegisteredNotificationHandler(UserRegisteredPublisher publisher)
    : INotificationHandler<UserRegisteredNotification>
{
    public Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken) =>
        publisher.PublishAsync(notification.UserId, cancellationToken);
}
