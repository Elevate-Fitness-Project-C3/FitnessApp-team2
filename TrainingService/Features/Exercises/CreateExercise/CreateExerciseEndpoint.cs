namespace TrainingService.Features.Exercises.CreateExercise;

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

public static class CreateExerciseEndpoint
{
    public static RouteGroupBuilder MapCreateExercise(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            [FromBody] CreateExerciseCommand command,
            IValidator<CreateExerciseCommand> validator,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            // 1. Validate request (returns HttpValidationProblemDetails)
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            // 2. Dispatch command to MediatR handler
            var result = await sender.Send(command, cancellationToken);

            // 3. Handle conflict (returns ProblemDetails for 409 Conflict)
            if (result.IsConflict)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Exercise Name Conflict",
                    detail: result.ErrorMessage,
                    instance: "/api/v1/exercises"
                );
            }

            // 4. Return 201 Created
            return Results.Created($"/api/v1/exercises/{result.Data!.Id}", result.Data);
        })
        .WithName("CreateExercise")
        .WithSummary("Create a new exercise definition")
        .WithDescription("Allows an Admin to create a unique exercise definition in the catalog (TR-A01).")
        .Produces<ExerciseResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}
