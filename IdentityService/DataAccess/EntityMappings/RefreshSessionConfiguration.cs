using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DataAccess.EntityMappings;

public sealed class RefreshSessionConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(session => session.Id);
        builder.Property(session => session.TokenHash).HasColumnType("char(64)").IsConcurrencyToken();
        builder.HasIndex(session => session.TokenHash).IsUnique();
        builder.HasOne(session => session.User).WithMany().HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
