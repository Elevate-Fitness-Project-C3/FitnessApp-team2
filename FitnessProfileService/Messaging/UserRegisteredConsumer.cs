using Gym.Contracts;
using MassTransit;

namespace FitnessProfileService.Messaging;

public sealed class UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger) : IConsumer<UserRegistered>
{
    public Task Consume(ConsumeContext<UserRegistered> context)
    {
        logger.LogInformation("FitnessProfile received registered user {UserId}", context.Message.UserId);
        return Task.CompletedTask;
    }
}
