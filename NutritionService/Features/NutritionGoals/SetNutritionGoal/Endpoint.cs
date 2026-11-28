using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutritionService.Common.Abstractions;
using NutritionService.Domain.Errors;

namespace NutritionService.Features.NutritionGoals.SetNutritionGoal;

public static class Endpoint
{
    public static RouteHandlerBuilder MapEndpoint(
        IEndpointRouteBuilder app)
    {
        return app.MapPut(
                "/api/v1/nutrition-goal",
                HandleAsync)
            .WithName("SetNutritionGoal")
            .WithTags("Nutrition Goals")
            .WithSummary("Set or change the user's nutrition goal")
           .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] SetNutritionGoalRequest request,
        ICurrentUser currentUser,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Results.Unauthorized();
        }

        var command = new SetNutritionGoalCommand(
            userId,
            request.NutritionGoal);

        var result = await sender.Send(
            command,
            cancellationToken);

        if (result.IsFailure)
        {

            if (result.Error == NutritionPreferenceErrors.ConcurrentCreate)
            {
                return Results.Problem(
                    title: result.Error.Code,
                    detail: result.Error.Description,
                    statusCode: StatusCodes.Status409Conflict);
            }

            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Results.NoContent();
    }
}
