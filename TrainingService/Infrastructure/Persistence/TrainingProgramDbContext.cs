using Microsoft.EntityFrameworkCore;
using TrainingService.Features.ProgramWorkout.Domain.Entities;
using TrainingProgramEntity = TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram;
using WorkoutEntity = TrainingService.Workout.Entities.Workout;

namespace TrainingService.Infrastructure.Persistence
{
    public class TrainingProgramDbContext : DbContext
    {
        public TrainingProgramDbContext(
            DbContextOptions<TrainingProgramDbContext> options)
            : base(options)
        {
        }

        public DbSet<TrainingProgramEntity> TrainingPrograms { get; set; }
        public DbSet<WorkoutEntity> Workouts { get; set; }
        public DbSet<ProgramWorkout> ProgramWorkouts { get; set; }
    }
}
