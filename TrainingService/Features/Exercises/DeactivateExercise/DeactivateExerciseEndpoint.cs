namespace TrainingService.Features.Exercises.DeactivateExercise;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

public static class DeactivateExerciseEndpoint
{
    public static RouteGroupBuilder MapDeactivateExercise(this RouteGroupBuilder group)
    {
        group.MapPatch("/{id:guid}/deactivate", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new DeactivateExerciseCommand(id);
            var result = await sender.Send(command, cancellationToken);

            if (result.IsNotFound)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Exercise Not Found",
                    detail: result.ErrorMessage,
                    instance: $"/api/v1/exercises/{id}/deactivate"
                );
            }

            return Results.NoContent();
        })
        .WithName("DeactivateExercise")
        .WithSummary("Deactivate an exercise definition")
        .WithDescription("Soft-deactivates an exercise definition so it cannot be added to newly published workouts (TR-A01 / AC 02 & AC 03).")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
