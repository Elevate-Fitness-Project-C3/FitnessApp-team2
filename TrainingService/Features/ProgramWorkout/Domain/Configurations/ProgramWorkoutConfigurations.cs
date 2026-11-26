using Microsoft.EntityFrameworkCore;

namespace TrainingService.Features.ProgramWorkout.Domain.Configurations
{
    public class ProgramWorkoutConfigurations : IEntityTypeConfiguration<Entities.ProgramWorkout>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Entities.ProgramWorkout> builder)
        {
            builder.HasKey(pw => pw.Id);

            builder.HasIndex(pw => new
            {
                pw.ProgramId,
                pw.WeekNumber,
                pw.DayNumber
            }).IsUnique();

            builder.Property(pw => pw.WeekNumber)
                .IsRequired();

            builder.Property(pw => pw.DayNumber)
                .IsRequired();

            builder.HasOne(pw => pw.Program)
                .WithMany(p => p.ProgramWorkouts)
                .HasForeignKey(pw => pw.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pw => pw.Workout)
                .WithMany()
                .HasForeignKey(pw => pw.WorkoutId);
        }
    }
}
