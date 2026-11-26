using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrainingService.Workout.Entities.Configurations
{
    public class WorkoutExerciseConfigurations : IEntityTypeConfiguration<WorkoutExercise>
    {
        public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
        {
            builder.ToTable("WorkoutExercises");

            builder.HasKey(we => we.Id);

            builder.Property(we => we.WorkoutId)
                .IsRequired();

            builder.Property(we => we.ExerciseId)
                .IsRequired();

            builder.Property(we => we.OrderIndex)
                .IsRequired();

            builder.Property(we => we.Sets)
                .IsRequired();

            // AC 03: Reject duplicate order values within a workout
            builder.HasIndex(we => new { we.WorkoutId, we.OrderIndex })
                .IsUnique();

            builder.HasOne(we => we.Workout)
                .WithMany(w => w.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(we => we.Exercise)
                .WithMany()
                .HasForeignKey(we => we.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
