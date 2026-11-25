using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrainingService.Workout.Entities.Configurations
{
    public class WorkoutConfigurations : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.ToTable("Workouts");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Enforce database-level uniqueness for workout title
            builder.HasIndex(w => w.Title)
                .IsUnique();

            builder.Property(w => w.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(w => w.Difficulty)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(w => w.EstimatedDurationMinutes)
                .IsRequired();

            builder.HasMany(w => w.WorkoutExercises)
                .WithOne(we => we.Workout)
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
