namespace TrainingService.Features.Exercises.GetExercises;

using MediatR;
using TrainingService.Domain.Enums;

public record GetExercisesQuery(
    bool? IsActive = null,
    TargetMuscleGroup? TargetMuscleGroup = null,
    string? Search = null) : IRequest<IReadOnlyList<ExerciseResponse>>;
