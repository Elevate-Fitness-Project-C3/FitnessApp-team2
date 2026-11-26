namespace TrainingService.Features.TrainingPrograms.DTOs
{
    public class PublishedProgramDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public int DurationWeeks { get; set; }
        public int DaysPerWeek { get; set; }
        public string AccessTier { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class PublishedProgramDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public int DurationWeeks { get; set; }
        public int DaysPerWeek { get; set; }
        public string AccessTier { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<ProgramSlotDTO> Schedule { get; set; } = new();
    }

    public class ProgramSlotDTO
    {
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }
        public ProgramWorkoutSummaryDTO? Workout { get; set; }
    }

    public class ProgramWorkoutSummaryDTO
    {
        public int WorkoutId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int EstimatedDurationMinutes { get; set; }
        public List<ProgramWorkoutExerciseDTO> Exercises { get; set; } = new();
    }

    public class ProgramWorkoutExerciseDTO
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
}
