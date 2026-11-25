using TrainingProgramEntity = TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram;
using WorkoutEntity = TrainingService.Workout.Entities.Workout;
        
namespace TrainingService.Features.ProgramWorkout.Domain.Entities
{
    public class ProgramWorkout
    {
        public int Id { get; set; }

        public int ProgramId { get; set; }

        public int WorkoutId { get; set; }

        public int WeekNumber { get; set; }

        public int DayNumber { get; set; }

        // Navigation Properties
        public TrainingProgramEntity Program { get; set; }

        public WorkoutEntity Workout { get; set; }
    }
}
