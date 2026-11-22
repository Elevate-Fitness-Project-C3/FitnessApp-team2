using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using MediatR;
using IdentityService.Features.Login;
using IdentityService.DataAccess;
using IdentityService.DataAccess.Context;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Features.Register.Validators;
using IdentityService.Services.Email;
using IdentityService.Services.JwtServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException("Configure ConnectionStrings:IdentityDatabase.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<UnitOfWork>();
        services.AddMediatR(typeof(ServiceRegistration).Assembly);
        services.AddAuthorization();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("registration", context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
        });
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = RegisterValidator.MinimumPasswordLength;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        }).AddRoles<IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddSignInManager();

        services.AddScoped<IEmailSender, EmailSender>();

        var jwtOption = configuration.GetRequiredSection("Jwt").Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt settings are missing.");
        if (string.IsNullOrWhiteSpace(jwtOption.Issuer) || string.IsNullOrWhiteSpace(jwtOption.Audience) ||
            jwtOption.DurationInMinutes <= 0 || jwtOption.RefreshDurationInDays <= 0)
            throw new InvalidOperationException("Configure the JWT issuer, audience and positive token lifetimes.");
        if (string.IsNullOrWhiteSpace(jwtOption.SecretKey) || Encoding.UTF8.GetByteCount(jwtOption.SecretKey) < 32)
            throw new InvalidOperationException("Set Jwt:SecretKey to a random signing key of at least 32 bytes.");

        services.AddSingleton(jwtOption);
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<LoginSessions>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOption.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtOption.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "roles",
                NameClaimType = "sub",

                ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
            };
        });

        return services;
    }
}
