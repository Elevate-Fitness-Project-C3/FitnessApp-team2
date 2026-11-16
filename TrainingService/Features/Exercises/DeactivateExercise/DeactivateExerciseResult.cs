namespace TrainingService.Features.Exercises.DeactivateExercise;

public class DeactivateExerciseResult
{
    public bool IsSuccess { get; private set; }
    public bool IsNotFound { get; private set; }
    public string? ErrorMessage { get; private set; }

    private DeactivateExerciseResult() { }

    public static DeactivateExerciseResult Success() =>
        new() { IsSuccess = true };

    public static DeactivateExerciseResult NotFound(string errorMessage) =>
        new()
        {
            IsNotFound = true,
            ErrorMessage = errorMessage
        };
}
