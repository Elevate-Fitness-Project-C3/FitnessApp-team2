namespace TrainingService.Features.Exercises.UpdateExercise;

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using TrainingService.Domain.Enums;

public static class UpdateExerciseEndpoint
{
    public static RouteGroupBuilder MapUpdateExercise(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateExerciseRequest request,
            IValidator<UpdateExerciseCommand> validator,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateExerciseCommand(
                id,
                request.Name,
                request.Instructions,
                request.TargetMuscleGroup,
                request.TrackingType);

            // 1. Validate command (returns RFC 7807 HttpValidationProblemDetails)
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            // 2. Dispatch command to MediatR handler
            var result = await sender.Send(command, cancellationToken);

            // 3. Handle Not Found (returns RFC 7807 ProblemDetails for 404)
            if (result.IsNotFound)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Exercise Not Found",
                    detail: result.ErrorMessage,
                    instance: $"/api/v1/exercises/{id}"
                );
            }

            // 4. Handle Conflict (returns RFC 7807 ProblemDetails for 409)
            if (result.IsConflict)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Exercise Name Conflict",
                    detail: result.ErrorMessage,
                    instance: $"/api/v1/exercises/{id}"
                );
            }

            // 5. Return 200 OK with updated exercise
            return Results.Ok(result.Data);
        })
        .WithName("UpdateExercise")
        .WithSummary("Update permitted descriptive fields of an exercise")
        .WithDescription("Allows an Admin to update descriptive fields (Name, Instructions, Muscle Group, Tracking Type) of an existing exercise (TR-A01).")
        .Produces<ExerciseResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}

public record UpdateExerciseRequest(
    string Name,
    string Instructions,
    TargetMuscleGroup TargetMuscleGroup,
    TrackingType TrackingType);
