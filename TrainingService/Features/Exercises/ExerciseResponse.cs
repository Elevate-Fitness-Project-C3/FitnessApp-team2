namespace TrainingService.Features.Exercises;

public record ExerciseResponse(
    Guid Id,
    string Name,
    string Instructions,
    string TargetMuscleGroup,
    string TrackingType,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
