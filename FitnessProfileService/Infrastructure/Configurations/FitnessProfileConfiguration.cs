using FitnessProfileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessProfileService.Common.Configurations
{
    public class FitnessProfileConfiguration
     : IEntityTypeConfiguration<FitnessProfile>
    {
        public void Configure(EntityTypeBuilder<FitnessProfile> builder)
        {
            // Primary Key
            builder.HasKey(fp => fp.UserId);

            // Properties
            builder.Property(fp => fp.UserId)
                .ValueGeneratedNever();


            builder.Property(fp => fp.AvailableDaysPerWeek)
                .HasMaxLength(7);


            builder.Property(fp => fp.CreatedAt)
                .IsRequired();

            builder.ToTable("FitnessProfiles", table =>
            {
                table.HasCheckConstraint(
                    "CK_FitnessProfiles_DateOfBirth",
                    "[DateOfBirth] > '1900-01-01' AND [DateOfBirth] < CAST(GETDATE() AS DATE)");
            });

            // Ignore Derived Property
            builder.Ignore(fp => fp.Age);



            // Relationship
            builder.HasMany(fp => fp.BodyMeasurements)
                .WithOne(bm => bm.FitnessProfile)
                .HasForeignKey(bm => bm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
