
using FitnessProfileService.Features.CreateFitnessProfile.Consumers;
using FitnessProfileService.Messaging.Constants;
using FitnessProfileService.Messaging.Exceptions;
using MassTransit;
using Microsoft.Extensions.Options;

namespace FitnessProfileService.Messaging.Configurations;

public static class MassTransitConfiguration
{
    public static void AddMessaging(this IServiceCollection services,
                                    IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetRequiredSection(RabbitMqSettings.SectionName));

        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<UserRegisteredConsumer>();

            bus.UsingRabbitMq((context, rabbit) =>
            {
                var settings = context
                    .GetRequiredService<IOptions<RabbitMqSettings>>()
                    .Value;

                rabbit.Host(settings.Host, "/", host =>
                {
                    host.Username(settings.Username);
                    host.Password(settings.Password);
                });

                rabbit.ReceiveEndpoint(
                    MessagingConstants.UserRegisteredQueue,
                    endpoint =>
                    {
                        endpoint.ConfigureConsumer<UserRegisteredConsumer>(
                            context);
                        endpoint.UseMessageRetry(retry =>
                        {
                            retry.Ignore<InvalidMessageException>();
                            retry.Ignore<BusinessException>();

                            retry.Handle<ConsumerProcessingException>();
                            
                            retry.Interval(3, TimeSpan.FromSeconds(5));
                            
                        });
                       
                    });
            });
        });
    }
}
