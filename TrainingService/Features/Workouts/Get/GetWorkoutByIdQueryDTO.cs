namespace TrainingService.Features.Workouts.Get
{
    public class WorkoutExerciseDetailsDTO
    {
        public int Id { get; set; }
        public Guid ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public int Sets { get; set; }
        public int? Reps { get; set; }
        public int? DurationSeconds { get; set; }
        public int? RestTimeSeconds { get; set; }
    }

    public class GetWorkoutByIdQueryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int EstimatedDurationMinutes { get; set; }
        public List<WorkoutExerciseDetailsDTO> Exercises { get; set; } = new();
    }
}
