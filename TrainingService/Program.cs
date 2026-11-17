using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Data;
using TrainingService.Features.Exercises;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Configure JSON options to serialize/deserialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Register standard RFC 7807 Problem Details service
builder.Services.AddProblemDetails();

// Register MediatR v10.0.1 for CQRS / Vertical Slice handlers
builder.Services.AddMediatR(typeof(Program));

// Register all FluentValidation validators automatically from this assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register TrainingDbContext with SQL Server
builder.Services.AddDbContext<TrainingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =================================================================================================
// TODO: [ID-06] Add Authentication & Authorization (Admin-role JWT authorization)
// To be implemented once the team aligns on the strategy (e.g., Asymmetric RS256 vs JWKS).
// =================================================================================================

var app = builder.Build();

// Ensure the database is created and schema is up-to-date on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TrainingDbContext>();
    db.Database.EnsureCreated();
}

// Global RFC 7807 problem details error handling
app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map Exercise Feature Slices
app.MapExerciseEndpoints();

app.Run();
