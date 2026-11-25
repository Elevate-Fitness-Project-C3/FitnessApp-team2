namespace TrainingService.Features.Exercises.GetExerciseById;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

public static class GetExerciseByIdEndpoint
{
    public static RouteGroupBuilder MapGetExerciseById(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetExerciseByIdQuery(id);
            var exercise = await sender.Send(query, cancellationToken);

            if (exercise is null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Exercise Not Found",
                    detail: $"Exercise with ID '{id}' was not found in the catalog.",
                    instance: $"/api/v1/exercises/{id}");
            }

            return Results.Ok(exercise);
        })
        .WithName("GetExerciseById")
        .WithSummary("Get exercise by ID")
        .WithDescription("Retrieves the full details of an exercise definition by its unique identifier (TR-A01).")
        .Produces<ExerciseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
