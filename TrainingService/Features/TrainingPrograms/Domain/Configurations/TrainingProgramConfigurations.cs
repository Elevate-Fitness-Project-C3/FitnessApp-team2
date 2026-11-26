using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingProgramEntity = TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram;

namespace TrainingService.TrainingProgram.Entities.Configrations
{
    public class TrainingProgramConfiguration : IEntityTypeConfiguration<TrainingProgramEntity>
    {
        public void Configure(EntityTypeBuilder<TrainingProgramEntity> builder)
        {
            builder.ToTable("TrainingPrograms");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Goal)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Level)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.DurationWeeks)
                .IsRequired();

            builder.Property(p => p.DaysPerWeek)
                .IsRequired();

            builder.Property(p => p.AccessTier)
                .IsRequired();

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(p => p.Status)
                .IsRequired();

            builder.HasMany(p => p.ProgramWorkouts)
                .WithOne(pw => pw.Program)
                .HasForeignKey(pw => pw.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
