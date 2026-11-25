namespace TrainingService.Features.Workouts.Get
{
    public class GetWorkoutByIdQueryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int EstimatedDurationMinutes { get; set; }
    }
}
