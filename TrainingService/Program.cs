using FluentValidation;
using TrainingService.Data;
using TrainingService.Features.Exercises;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Register standard Problem Details service
builder.Services.AddProblemDetails();

// Register MediatR for CQRS / Vertical Slice handlers
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Register all FluentValidation validators automatically from this assembly
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register TrainingDbContext (Provider will be configured based on team's DB selection)
builder.Services.AddDbContext<TrainingDbContext>();

var app = builder.Build();

// Global problem details error handling
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
