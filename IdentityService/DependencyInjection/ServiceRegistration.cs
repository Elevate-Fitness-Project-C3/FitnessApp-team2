using System.Threading.RateLimiting;
using IdentityService.DataAccess;
using IdentityService.DataAccess.Context;
using IdentityService.DataAccess.Repositories;
using IdentityService.Common.ResultPattern;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.Behaviors;
using IdentityService.Features.Register.Commands;
using IdentityService.Features.Register.Orchestrators;
using IdentityService.Features.Register.Validators;
using IdentityService.Features.Register.ViewModels;
using IdentityService.Services.Email;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DependencyInjection;

public static class ServiceRegistration
{
    public static WebApplicationBuilder AddIdentityService(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var connectionString = builder.Configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException("Configure ConnectionStrings:IdentityDatabase.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddMediatR(typeof(RegisterOrchestrator).Assembly);
        services.AddTransient<IPipelineBehavior<RegisterCommand, RequestResponse<RegisterViewModel>>, RegisterBehavior>();

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = RegisterValidator.MinimumPasswordLength;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<IEmailSender, EmailSender>();
        services.AddIdentityMessaging(builder.Configuration);
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("registration", context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1)
                }));
        });
        return builder;
    }
}
