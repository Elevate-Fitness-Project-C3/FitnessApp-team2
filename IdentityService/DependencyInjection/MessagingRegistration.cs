using IdentityService.DataAccess.Context;
using IdentityService.Messaging;
using MassTransit;
using Microsoft.Extensions.Options;

namespace IdentityService.DependencyInjection;

public static class MessagingRegistration
{
    public static IServiceCollection AddIdentityMessaging(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Host), "RabbitMq:Host is required.")
            .Validate(options => options.Port > 0, "RabbitMq:Port must be between 1 and 65535.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.VirtualHost), "RabbitMq:VirtualHost is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Username), "RabbitMq:Username is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Password), "RabbitMq:Password is required.")
            .ValidateOnStart();

        services.AddMassTransit(bus =>
        {
            bus.AddEntityFrameworkOutbox<ApplicationDbContext>(outbox =>
            {
                outbox.UseSqlServer();
                outbox.UseBusOutbox();
                outbox.QueryDelay = TimeSpan.FromSeconds(1);
            });

            bus.UsingRabbitMq((context, transport) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                transport.Host(options.Host, options.Port, options.VirtualHost, host =>
                {
                    host.Username(options.Username);
                    host.Password(options.Password);
                });
            });
        });

        services.AddScoped<UserRegisteredPublisher>();
        return services;
    }
}
