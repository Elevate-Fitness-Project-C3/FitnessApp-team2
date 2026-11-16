namespace TrainingService.Features.Exercises;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TrainingService.Features.Exercises.CreateExercise;

public static class ExerciseEndpoints
{
    public static IEndpointRouteBuilder MapExerciseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/exercises")
            .WithTags("Exercises");

        // Register feature slices
        group.MapCreateExercise();

        return app;
    }
}
