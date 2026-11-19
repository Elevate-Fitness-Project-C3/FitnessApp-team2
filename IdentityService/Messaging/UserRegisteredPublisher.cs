using Fitness.Contracts.Identity.V1;
using MassTransit;

namespace IdentityService.Messaging;

public sealed class UserRegisteredPublisher(IPublishEndpoint publisher)
{
    public Task PublishAsync(string userId, CancellationToken cancellationToken = default)
    {
        var message = new UserRegistered(Guid.NewGuid(), userId, DateTimeOffset.UtcNow);
        return publisher.Publish(message, context =>
        {
            context.MessageId = message.EventId;
        }, cancellationToken);
    }
}
