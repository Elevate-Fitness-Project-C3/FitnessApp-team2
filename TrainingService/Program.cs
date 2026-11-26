using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrainingService.Data;
using TrainingService.Features.Exercises;
using TrainingService.Features.ProgramWorkout.Create;
using TrainingService.Infrastructure.Repositories.Classes;
using TrainingService.Repositories.Interfaces;
using TrainingService.TrainingProgram.EndPoints;
using TrainingService.Workout.EndPoints;

var builder = WebApplication.CreateBuilder(args);

// Register controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register database contexts
builder.Services.AddDbContext<
    TrainingService.Infrastructure.Persistence.TrainingProgramDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<TrainingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped(
    typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register orchestrators
builder.Services.AddScoped<CreateProgramWorkoutOrchestrator>();

// Configure JSON enum serialization
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

// Register RFC 7807 Problem Details
builder.Services.AddProblemDetails();

// Register MediatR
builder.Services.AddMediatR(typeof(Program).Assembly);

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configure JWT Authentication & Authorization
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = builder.Configuration["JWT_SECRET_KEY"]
    ?? (!string.IsNullOrWhiteSpace(jwtSection["SecretKey"]) ? jwtSection["SecretKey"] : null);

if (string.IsNullOrWhiteSpace(secretKey))
{
    // Read from root .env file if running in local development
    var rootDir = Directory.GetParent(builder.Environment.ContentRootPath)?.FullName ?? builder.Environment.ContentRootPath;
    var envFile = Path.Combine(rootDir, ".env");
    if (File.Exists(envFile))
    {
        foreach (var line in File.ReadAllLines(envFile))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("JWT_SECRET_KEY=", StringComparison.OrdinalIgnoreCase))
            {
                secretKey = trimmed["JWT_SECRET_KEY=".Length..].Trim();
                break;
            }
        }
    }
}

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("Jwt:SecretKey configuration or JWT_SECRET_KEY environment variable is missing.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"] ?? "fittnesservices",

            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"] ?? "angular",

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),

            RoleClaimType = "roles",
            NameClaimType = "sub"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireAssertion(context =>
        context.User.IsInRole("Admin") ||
        context.User.HasClaim(c => (c.Type == "roles" || c.Type == "role" || c.Type == System.Security.Claims.ClaimTypes.Role) && c.Value == "Admin")));

    options.AddPolicy("PremiumOnly", policy => policy.RequireAssertion(context =>
        context.User.IsInRole("Premium") ||
        context.User.HasClaim(c => (c.Type == "roles" || c.Type == "role" || c.Type == System.Security.Claims.ClaimTypes.Role) && c.Value == "Premium")));
});

var app = builder.Build();

// Apply pending database migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<TrainingService.Infrastructure.Persistence.TrainingProgramDbContext>();

    db.Database.Migrate();
}

// Global error handling
app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Exercise endpoints
app.MapExerciseEndpoints();

// Training Program endpoints
app.MapCreateTrainingProgramEndPoint();

// Workout endpoints
app.MapCreateWorkoutEndPoint();

// Program Workout endpoints
app.MapCreateProgramWorkoutEndPoint();

app.Run();
