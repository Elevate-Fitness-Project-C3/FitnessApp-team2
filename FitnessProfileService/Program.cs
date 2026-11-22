using FitnessProfileService.Domain.Contracts;
using FitnessProfileService.Features.Common;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Endpoints;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Validators;
using FitnessProfileService.Infrastructure.Persistence;
using FitnessProfileService.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProfileDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(
    typeof(CompleteFitnessOnboardingOrchestratorValidator).Assembly);
builder.Services.AddScoped( typeof(IGenericRepository<>),typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>( );
// MediatR
builder.Services.AddMediatR(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
CompleteFitnessOnboardingEndpoint.MapPostCancelEnrollmentEndpoint(app);
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
