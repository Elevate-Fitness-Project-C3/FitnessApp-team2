namespace TrainingService.Features.Exercises.CreateExercise;

public class CreateExerciseResult
{
    public bool IsSuccess { get; private set; }
    public bool IsConflict { get; private set; }
    public ExerciseResponse? Data { get; private set; }
    public string? ErrorMessage { get; private set; }

    private CreateExerciseResult() { }

    public static CreateExerciseResult Success(ExerciseResponse data) =>
        new()
        {
            IsSuccess = true,
            Data = data
        };

    public static CreateExerciseResult Conflict(string errorMessage) =>
        new()
        {
            IsConflict = true,
            ErrorMessage = errorMessage
        };
}
