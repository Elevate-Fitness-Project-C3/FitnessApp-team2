namespace TrainingService.Features.Exercises.CreateExercise;

using MediatR;
using TrainingService.Common.Results;
using TrainingService.Domain.Enums;

public record CreateExerciseCommand(
    string Name,
    string Instructions,
    TargetMuscleGroup TargetMuscleGroup,
    TrackingType TrackingType) : IRequest<Result<Guid>>;
