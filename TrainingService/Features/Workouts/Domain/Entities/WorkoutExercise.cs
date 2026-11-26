using TrainingService.Domain.Entities;

namespace TrainingService.Workout.Entities
{
    public class WorkoutExercise
    {
        public int Id { get; set; }

        public int WorkoutId { get; set; }

        public Guid ExerciseId { get; set; }

        public int OrderIndex { get; set; }

        public int Sets { get; set; }

        public int? Reps { get; set; }

        public int? DurationSeconds { get; set; }

        public int? RestTimeSeconds { get; set; }

        // Navigation properties
        public Workout Workout { get; set; } = null!;

        public Exercise? Exercise { get; set; }
    }
}
