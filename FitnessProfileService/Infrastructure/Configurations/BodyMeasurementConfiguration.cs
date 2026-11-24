using FitnessProfileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessProfileService.Infrastructure.Configurations
{
    public class BodyMeasurementConfiguration
        : IEntityTypeConfiguration<BodyMeasurement>
    {
        public void Configure(EntityTypeBuilder<BodyMeasurement> builder)
        {
            builder.ToTable("BodyMeasurements", table =>
            {
                table.HasCheckConstraint(
                    "CK_BodyMeasurements_HeightCm",
                    "[HeightCm] > 0");

                table.HasCheckConstraint(
                    "CK_BodyMeasurements_WeightKg",
                    "[WeightKg] > 0");
            });

            builder.HasKey(b => b.MeasurementId);

            builder.Property(b => b.MeasurementId)
                .ValueGeneratedOnAdd();

            builder.Property(b => b.UserId)
                .IsRequired();

            builder.Property(b => b.HeightCm)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(b => b.WeightKg)
                .HasPrecision(5, 2)
                .IsRequired();

            // Index
            builder.HasIndex(b => b.UserId);
            builder.HasIndex(b => b.RecordedAt);
        }
    }
}