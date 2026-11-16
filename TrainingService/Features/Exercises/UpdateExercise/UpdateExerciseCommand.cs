namespace TrainingService.Features.Exercises.UpdateExercise;

using MediatR;
using TrainingService.Domain.Enums;

public record UpdateExerciseCommand(
    Guid Id,
    string Name,
    string Instructions,
    TargetMuscleGroup TargetMuscleGroup,
    TrackingType TrackingType) : IRequest<UpdateExerciseResult>;
