using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Workout.Commands;
using TrainingService.Workout.ViewModels;
using TrainingService.Validation;

namespace TrainingService.Workout.EndPoints
{
    public static class CreateWorkoutEndPoint
    {
        public static void MapCreateWorkoutEndPoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/workouts", async (CreateWorkoutCommand command, IMediator mediator) =>
            {
                try
                {
                    var WorkoutId = await mediator.Send(command);
                    return Results.Created($"/api/workouts/{WorkoutId}", WorkoutId);
                }
                catch (RequestValidationException exception)
                {
                    return Results.ValidationProblem(exception.Errors);
                }
            })
            .WithName("CreateWorkout")
            .WithTags("Workouts");

            
        }
    }
}
