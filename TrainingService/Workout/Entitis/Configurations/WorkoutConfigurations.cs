using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrainingService.Workout.Entitis.Configurations
{
    public class WorkoutConfigurations : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Title)
                .IsRequired();
            builder.Property(w => w.Description)
                .IsRequired();
            builder.Property(w => w.Difficulty)
                .IsRequired();
            builder.Property(w => w.EstimatedDurationMinutes)
                .IsRequired();
        }
    }
}
