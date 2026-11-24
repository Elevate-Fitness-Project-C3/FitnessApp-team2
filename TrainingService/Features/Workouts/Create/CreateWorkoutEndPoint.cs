using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Workouts.Get;
using TrainingService.TrainingProgram.Queries;
using TrainingService.Validation;
using TrainingService.Workout.Commands;
using TrainingService.Workout.DTOs;

namespace TrainingService.Workout.EndPoints
{
    public static class CreateWorkoutEndPoint
    {
        public static void MapWorkoutEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/workouts")
                .WithTags("Workouts");

            // POST /api/workouts - Create Workout (TR-A02)
            group.MapPost("", async ([FromBody] CreateWorkoutCommand command, IMediator mediator) =>
            {
                try
                {
                    var workoutId = await mediator.Send(command);
                    return Results.Created($"/api/workouts/{workoutId}", workoutId);
                }
                catch (RequestValidationException exception)
                {
                    return Results.ValidationProblem(exception.Errors);
                }
            })
            .WithName("CreateWorkout")
            .Produces<int>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

            // GET /api/workouts/{id} - Get Workout by ID with ordered exercises
            group.MapGet("{id:int}", async (int id, IMediator mediator) =>
            {
                var workout = await mediator.Send(new GetWorkoutByIdQuery(id));
                return workout is not null ? Results.Ok(workout) : Results.NotFound();
            })
            .WithName("GetWorkoutById")
            .Produces<GetWorkoutByIdQueryDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // GET /api/workouts - List Workouts with optional filter
            group.MapGet("", async ([FromQuery] string? difficulty, [FromQuery] string? search, IMediator mediator) =>
            {
                var workouts = await mediator.Send(new GetWorkoutsQuery(difficulty, search));
                return Results.Ok(workouts);
            })
            .WithName("GetWorkouts")
            .Produces<List<GetWorkoutByIdQueryDTO>>(StatusCodes.Status200OK);

            // PUT /api/workouts/{id} - Update Workout (guarded by AC 05)
            group.MapPut("{id:int}", async (int id, [FromBody] UpdateWorkoutRequest request, IMediator mediator) =>
            {
                try
                {
                    var command = new UpdateWorkoutCommand(
                        id,
                        request.Title,
                        request.Description,
                        request.Difficulty,
                        request.EstimatedDurationMinutes,
                        request.Exercises);

                    await mediator.Send(command);
                    return Results.NoContent();
                }
                catch (RequestValidationException exception)
                {
                    return Results.ValidationProblem(exception.Errors);
                }
                catch (KeyNotFoundException exception)
                {
                    return Results.NotFound(new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = exception.Message,
                        Status = StatusCodes.Status404NotFound
                    });
                }
                catch (WorkoutPublishedException exception)
                {
                    return Results.Conflict(new ProblemDetails
                    {
                        Title = "Conflict",
                        Detail = exception.Message,
                        Status = StatusCodes.Status409Conflict
                    });
                }
            })
            .WithName("UpdateWorkout")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

            // DELETE /api/workouts/{id} - Delete Workout (guarded by AC 05)
            group.MapDelete("{id:int}", async (int id, IMediator mediator) =>
            {
                try
                {
                    await mediator.Send(new DeleteWorkoutCommand(id));
                    return Results.NoContent();
                }
                catch (KeyNotFoundException exception)
                {
                    return Results.NotFound(new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = exception.Message,
                        Status = StatusCodes.Status404NotFound
                    });
                }
                catch (WorkoutPublishedException exception)
                {
                    return Results.Conflict(new ProblemDetails
                    {
                        Title = "Conflict",
                        Detail = exception.Message,
                        Status = StatusCodes.Status409Conflict
                    });
                }
            })
            .WithName("DeleteWorkout")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        }

        // Retained for Program.cs compatibility
        public static void MapCreateWorkoutEndPoint(this IEndpointRouteBuilder app)
        {
            app.MapWorkoutEndpoints();
        }
    }

    public record UpdateWorkoutRequest(
        string Title,
        string Description,
        string Difficulty,
        int EstimatedDurationMinutes,
        List<WorkoutExerciseItem> Exercises
    );
}
