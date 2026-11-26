using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Behaviors;
using NutritionService.Domain.Repositories;
using NutritionService.Infrastructure.Authentication;
using NutritionService.Infrastructure.Identifiers;
using NutritionService.Persistence;
using NutritionService.Persistence.Repositories;

namespace NutritionService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNutritionServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
                                   .GetConnectionString("NutritionDb")
                               ?? throw new InvalidOperationException(
                                   "NutritionDb connection string is missing.");

        services.AddDbContext<NutritionDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<NutritionDbContext>());

        services.AddScoped<
            INutritionPreferenceRepository,
            NutritionPreferenceRepository>();

        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(mediatrConfiguration =>
        {
            mediatrConfiguration.RegisterServicesFromAssembly(assembly);

            mediatrConfiguration.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUser, CurrentUser>();

        var workerId = configuration.GetValue<int>("Snowflake:WorkerId");

        services.AddSingleton<ISnowflakeIdGenerator>(
            _ => new SnowflakeIdGenerator(workerId));

        return services;
    }
}