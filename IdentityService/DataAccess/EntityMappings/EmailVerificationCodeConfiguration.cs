using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DataAccess.EntityMappings;

public sealed class EmailVerificationCodeConfiguration : IEntityTypeConfiguration<EmailVerificationCode>
{
    public void Configure(EntityTypeBuilder<EmailVerificationCode> code)
    {
        code.HasKey(x => x.UserId);
        code.Property(x => x.CodeHash).HasMaxLength(512).IsRequired();
        code.Property(x => x.Version).IsConcurrencyToken();
        code.HasOne(x => x.User).WithOne().HasForeignKey<EmailVerificationCode>(x => x.UserId);
    }
}
