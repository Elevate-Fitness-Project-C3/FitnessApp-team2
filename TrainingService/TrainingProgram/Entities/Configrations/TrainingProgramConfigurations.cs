using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrainingService.TrainingProgram.Entities.Configrations
{

    public class TrainingProgramConfiguration : IEntityTypeConfiguration<TrainingProgram>
    {
        public void Configure(EntityTypeBuilder<TrainingProgram> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Goal)
                .IsRequired();

            builder.Property(p => p.Level)
                .IsRequired();

            builder.Property(p => p.DurationWeeks)
                .IsRequired();

            builder.Property(p => p.DaysPerWeek)
                .IsRequired();

            builder.Property(p => p.AccessTier)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

           
        }
    }

}
