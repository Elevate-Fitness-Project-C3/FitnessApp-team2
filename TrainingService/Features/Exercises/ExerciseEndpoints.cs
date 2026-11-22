namespace TrainingService.Features.Exercises;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TrainingService.Features.Exercises.CreateExercise;
using TrainingService.Features.Exercises.DeactivateExercise;
using TrainingService.Features.Exercises.GetExerciseById;
using TrainingService.Features.Exercises.GetExercises;
using TrainingService.Features.Exercises.UpdateExercise;

public static class ExerciseEndpoints
{
    public static IEndpointRouteBuilder MapExerciseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/exercises")
            .WithTags("Exercises");

        // Feature slices
        group.MapCreateExercise();
        group.MapGetExercises();
        group.MapGetExerciseById();
        group.MapUpdateExercise();
        group.MapDeactivateExercise();

        return app;
    }
}
