using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingProgramEntity = TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram;

namespace TrainingService.TrainingProgram.Entities.Configrations
{

    public class TrainingProgramConfiguration : IEntityTypeConfiguration<TrainingProgramEntity>
    {
        public void Configure(EntityTypeBuilder<TrainingProgramEntity> builder)
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
