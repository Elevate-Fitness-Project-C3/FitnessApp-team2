using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Data;
using TrainingService.Infrastructure.Repositories.Classes;
using TrainingService.Repositories.Interfaces;
using TrainingService.TrainingProgram.EndPoints;
using TrainingService.Workout.EndPoints;
using TrainingService.Features.ProgramWorkout.Create;
using TrainingService.Features.Exercises;

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

// TODO: [ID-06] Add Authentication & Authorization

var app = builder.Build();

// Ensure database creation
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<TrainingDbContext>();

    db.Database.EnsureCreated();
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
