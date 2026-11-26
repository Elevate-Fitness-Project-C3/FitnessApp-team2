using static TrainingService.TrainingProgram.Enums.Enums;
using ProgramWorkoutEntity = TrainingService.Features.ProgramWorkout.Domain.Entities.ProgramWorkout;

namespace TrainingService.Features.TrainingPrograms.Domain.Entities
{
    public class TrainingProgram
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Goal { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public int DurationWeeks { get; set; }

        public int DaysPerWeek { get; set; }

        public AccessTier AccessTier { get; set; }

        public string Description { get; set; } = string.Empty;

        public ProgramStatus Status { get; set; } = ProgramStatus.Draft;

        // Navigation collection for scheduled program workouts (TR-U01)
        public ICollection<ProgramWorkoutEntity> ProgramWorkouts { get; set; } = new List<ProgramWorkoutEntity>();
    }
}
