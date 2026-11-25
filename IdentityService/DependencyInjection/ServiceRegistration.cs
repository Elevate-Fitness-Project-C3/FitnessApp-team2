using System.Text;
using System.Threading.RateLimiting;
using IdentityService.DataAccess;
using IdentityService.DataAccess.Context;
using IdentityService.DataAccess.Repositories;
using IdentityService.Domain.Entities;
using IdentityService.Services.Email;
using IdentityService.Services.JwtServices;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        // 
        var connectionString = configuration.GetConnectionString("IdentityDatabase")
            ?? throw new InvalidOperationException("Configure ConnectionStrings:IdentityDatabase.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        // identity config 
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.SignIn.RequireConfirmedEmail = true;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager();
        // services config
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<UnitOfWork>();
        services.AddMediatR(typeof(ServiceRegistration).Assembly);
        services.AddAuthorization();


        // jwt config 

        var jwtOption = configuration.GetRequiredSection("Jwt").Get<JwtOptions>()
       ?? throw new InvalidOperationException("Jwt settings are missing.");

        services.AddSingleton(jwtOption);

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
