using Demo.Security.Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Security.Infrastructure.Persistence.Config
{
    public sealed class PasswordResetConfig : IEntityTypeConfiguration<PasswordReset>
    {
        public void Configure(EntityTypeBuilder<PasswordReset> b)
        {
            b.ToTable("PasswordResets");
            b.HasKey(x => x.Id);

            b.Property(x => x.TokenHash).HasMaxLength(88).IsRequired(); // Base64 de SHA256 (44) - dejamos margen
            b.Property(x => x.ExpiresAt).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UsedAt);

            b.HasIndex(x => new { x.UserId, x.TokenHash }).IsUnique();
            b.HasIndex(x => x.ExpiresAt);

            b.HasOne<Domain.Users.User>()
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
