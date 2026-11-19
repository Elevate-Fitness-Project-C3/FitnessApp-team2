namespace TrainingService.Workout.Entitis
{
    public class Workout
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string EstimatedDurationMinutes { get; set; } = string.Empty;

    }
}
