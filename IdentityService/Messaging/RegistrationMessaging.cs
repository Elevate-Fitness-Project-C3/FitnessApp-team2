using MassTransit;
using IdentityService.DataAccess.Context;

namespace IdentityService.Messaging;

public static class RegistrationMessaging
{
    public static void AddRegistrationMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetRequiredSection("RabbitMq");
        services.AddMassTransit(bus =>
        {
            bus.AddEntityFrameworkOutbox<ApplicationDbContext>(options =>
            {
                options.UseSqlServer();
                options.UseBusOutbox();
            });
            bus.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(settings["Host"]!, "/", host =>
                {
                    host.Username(settings["Username"]!);
                    host.Password(settings["Password"]!);
                });
            });
        });
    }
}
