using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Features.ProgramWorkout.Domain.Entities;
using TrainingProgramEntity = TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram;
using WorkoutEntity = TrainingService.Workout.Entities.Workout;
using TrainingService.Workout.Entities;

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
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<ProgramWorkout> ProgramWorkouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainingProgramDbContext).Assembly);

            // Ensure Exercises table is excluded from TrainingProgramDbContext migrations
            // because it is maintained independently in the shared database
            modelBuilder.Entity<Exercise>(b =>
            {
                b.ToTable("Exercises", t => t.ExcludeFromMigrations());
            });
        }
    }
}
