using FluentValidation;
using MediatR;
using TrainingService.Data;
using TrainingService.Features.Exercises;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Register standard RFC 7807 Problem Details service
builder.Services.AddProblemDetails();

// Register MediatR v10.0.1 for CQRS / Vertical Slice handlers
builder.Services.AddMediatR(typeof(Program));

// Register all FluentValidation validators automatically from this assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register TrainingDbContext (Provider will be configured based on team's DB selection)
builder.Services.AddDbContext<TrainingDbContext>();

// =================================================================================================
// TODO: [ID-06] Add Authentication & Authorization (Admin-role JWT authorization)
// To be implemented once the team aligns on the strategy (e.g., Asymmetric RS256 vs JWKS).
// =================================================================================================

var app = builder.Build();

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
