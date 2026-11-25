namespace TrainingService.Workout.Entities
{
    public class Workout
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int EstimatedDurationMinutes { get; set; }

        // Navigation property for ordered exercise entries (TR-A02)
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }
}
