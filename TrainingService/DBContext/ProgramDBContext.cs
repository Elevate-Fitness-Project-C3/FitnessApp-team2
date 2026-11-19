using Microsoft.EntityFrameworkCore;

namespace TrainingService.DBContext
{
    public class TrainingProgramDbContext : DbContext
    {
        public TrainingProgramDbContext(
            DbContextOptions<TrainingProgramDbContext> options)
            : base(options)
        {
        }

        public DbSet<TrainingProgram.Entities.TrainingProgram> TrainingPrograms { get; set; }
        public DbSet<Workout.Entitis.Workout> Workouts { get; set; }
    }
}
