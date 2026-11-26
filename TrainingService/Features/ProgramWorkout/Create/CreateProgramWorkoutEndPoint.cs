namespace TrainingService.Features.ProgramWorkout.Create;

public static class CreateProgramWorkoutEndPoint
{
    public static void MapCreateProgramWorkoutEndPoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/program-workouts", async (
            CreateProgramWorkoutCommand command,
            CreateProgramWorkoutOrchestrator orchestrator,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var programWorkoutId = await orchestrator.Execute(command, cancellationToken);

                return Results.Created(
                    $"/api/program-workouts/{programWorkoutId}",
                    programWorkoutId);
            }
            catch (KeyNotFoundException exception)
            {
                return Results.NotFound(new { error = exception.Message });
            }
            catch (ArgumentException exception)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
            catch (InvalidOperationException exception)
            {
                return Results.Conflict(new { error = exception.Message });
            }
        })
        .WithName("CreateProgramWorkout")
        .WithTags("ProgramWorkouts")
        .RequireAuthorization("AdminOnly");
    }
}
