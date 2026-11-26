namespace TrainingService.Workout.ViewModels
{
    public class CreateWorkoutViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; } 
        public string Difficulty { get; set; } 
        public string EstimatedDurationMinutes { get; set; } 
    }
}
