using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NutritionService.Common.Abstractions;
using NutritionService.Common.Behaviors;
using NutritionService.Domain.Repositories;
using NutritionService.Infrastructure.Authentication;
using NutritionService.Infrastructure.Identifiers;
using NutritionService.Persistence;
using NutritionService.Persistence.Repositories;
using System.Text;

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
        var jwtSection = configuration.GetRequiredSection("Jwt");

        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException(
                "Jwt:SecretKey is required. " +
                "Set the environment variable Jwt__SecretKey " +
                "or configure it via user secrets: " +
                "dotnet user-secrets set \"Jwt:SecretKey\" \"<value>\"");

        var issuer   = jwtSection["Issuer"]   ?? "fittnesservices";
        var audience = jwtSection["Audience"] ?? "angular";

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidIssuer              = issuer,

                    ValidateAudience         = true,
                    ValidAudience            = audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),

                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero,

                    NameClaimType            = "sub",
                    RoleClaimType            = "roles",

                    ValidAlgorithms          = [SecurityAlgorithms.HmacSha256]
                };
            });

        return services;
    }
}
