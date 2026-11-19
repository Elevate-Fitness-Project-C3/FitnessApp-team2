using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DataAccess.EntityMappings;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> user)
    {
        user.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        user.Property(x => x.Email).IsRequired();
        user.Property(x => x.NormalizedEmail).IsRequired();
        user.HasIndex(x => x.NormalizedEmail).HasDatabaseName("EmailIndex").IsUnique();
    }
}
