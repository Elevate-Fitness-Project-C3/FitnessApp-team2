namespace TrainingService.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercises");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);

        // Ensure unique exercise names across the catalog
        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.Property(e => e.Instructions)
            .IsRequired()
            .HasMaxLength(2000);

        // Store enums as string representations for readability and schema robustness
        builder.Property(e => e.TargetMuscleGroup)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.TrackingType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Index on IsActive for high-performance catalog queries
        builder.HasIndex(e => e.IsActive);

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();

        builder.Property(e => e.UpdatedAtUtc)
            .IsRequired(false);
    }
}
