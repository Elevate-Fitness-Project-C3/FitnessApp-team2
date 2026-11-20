using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using MediatR;
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
            options.SignIn.RequireConfirmedEmail = true;
        }).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<IEmailSender, EmailSender>();

        // jwt configuration 
        var jwtOption = configuration.GetRequiredSection("Jwt").Get<JwtOptions>()
         ?? throw new InvalidOperationException("Jwt settings are missing.");

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
                IssuerSigningKey = string.IsNullOrEmpty(jwtOption.SecretKey) ? null :
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SecretKey)),

                ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
            };
        });

        return services;
    }
}
