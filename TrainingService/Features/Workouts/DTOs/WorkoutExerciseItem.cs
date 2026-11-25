namespace TrainingService.Workout.DTOs
{
    public record WorkoutExerciseItem(
        Guid ExerciseId,
        int OrderIndex,
        int Sets,
        int? Reps = null,
        int? DurationSeconds = null,
        int? RestTimeSeconds = null
    );
}
