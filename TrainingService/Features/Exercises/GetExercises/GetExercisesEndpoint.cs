namespace TrainingService.Features.Exercises.GetExercises;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TrainingService.Domain.Enums;

public static class GetExercisesEndpoint
{
    public static RouteGroupBuilder MapGetExercises(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] GetExercisesParameters parameters,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetExercisesQuery(
                parameters.IsActive,
                parameters.TargetMuscleGroup,
                parameters.Search);

            var exercises = await sender.Send(query, cancellationToken);

            return Results.Ok(exercises);
        })
        .WithName("GetExercises")
        .WithSummary("List and filter exercises in the catalog")
        .WithDescription("Retrieves exercises with optional filters for active status, target muscle group, and search keyword (TR-A01).")
        .Produces<IReadOnlyList<ExerciseResponse>>(StatusCodes.Status200OK);

        return group;
    }
}

public record GetExercisesParameters(
    bool? IsActive = null,
    TargetMuscleGroup? TargetMuscleGroup = null,
    string? Search = null);
