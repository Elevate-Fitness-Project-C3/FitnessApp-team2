using FitnessProfileService.FitnessProfileService.Features.CreateFitnessProfile.Commands;
using FitnessProfileService.Messaging.Exceptions;
using FitnessProfileService.Messaging.Extensions;
using Gym.Contracts;
using MassTransit;
using MediatR;

namespace FitnessProfileService.Features.CreateFitnessProfile.Consumers
{
    public class UserRegisteredConsumer(IMediator mediator, ILogger<UserRegisteredConsumer> logger) : IConsumer<UserRegistered>
    {
        public async Task Consume(ConsumeContext<UserRegistered> context)
        { var userId = context.Message.UserId;
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                throw new InvalidMessageException(
                    $"Failed to process UserRegistered event. " +
                    $"The received UserId '{userId}' is not a valid GUID.");
            }
           var result = await mediator.Send( new CreateFitnessProfileCommand(parsedUserId),
                                             context.CancellationToken);
            if(result is null)
                throw new ConsumerProcessingException($"Failed to process UserRegistered event for UserId '{userId}'.");
            result.ThrowIfFailed();
            
            logger.LogInformation("FitnessProfile received registered user {UserId}", context.Message.UserId);

        }
    }
}
