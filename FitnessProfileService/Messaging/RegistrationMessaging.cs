using MassTransit;

namespace FitnessProfileService.Messaging;

public static class RegistrationMessaging
{
    public static void AddRegistrationMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetRequiredSection("RabbitMq");
        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<UserRegisteredConsumer>();
            bus.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(settings["Host"]!, "/", host =>
                {
                    host.Username(settings["Username"]!);
                    host.Password(settings["Password"]!);
                });
                rabbit.ReceiveEndpoint("fitness-profile-user-registered", endpoint =>
                    endpoint.ConfigureConsumer<UserRegisteredConsumer>(context));
            });
        });
    }
}
