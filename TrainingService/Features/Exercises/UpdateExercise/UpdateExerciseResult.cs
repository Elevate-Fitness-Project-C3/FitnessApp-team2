namespace TrainingService.Features.Exercises.UpdateExercise;

public class UpdateExerciseResult
{
    public bool IsSuccess { get; private set; }
    public bool IsNotFound { get; private set; }
    public bool IsConflict { get; private set; }
    public ExerciseResponse? Data { get; private set; }
    public string? ErrorMessage { get; private set; }

    private UpdateExerciseResult() { }

    public static UpdateExerciseResult Success(ExerciseResponse data) =>
        new()
        {
            IsSuccess = true,
            Data = data
        };

    public static UpdateExerciseResult NotFound(string errorMessage) =>
        new()
        {
            IsNotFound = true,
            ErrorMessage = errorMessage
        };

    public static UpdateExerciseResult Conflict(string errorMessage) =>
        new()
        {
            IsConflict = true,
            ErrorMessage = errorMessage
        };
}
