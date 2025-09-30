using Demo.Security.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Security.Infrastructure.Persistence.Config
{
    public sealed class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.ToTable("Users");
            b.HasKey(x => x.Id);

            b.OwnsOne(x => x.Email, nb =>
            {
                nb.Property(e => e.Value)
                  .HasColumnName("Email")
                  .HasMaxLength(256)
                  .IsRequired();

                nb.HasIndex(e => e.Value).IsUnique();
            });

            b.Property(x => x.UserName).HasMaxLength(100).IsRequired();
            b.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt);

            // 🔧 Configurar many-to-many con nombres de columnas exactos: UserId / RoleId
            b.HasMany(u => u.Roles)
             .WithMany()
             .UsingEntity<Dictionary<string, object>>(
                "UserRoles",
                right => right  // lado Role
                    .HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId")          // <<--- nombre real en BD
                    .HasPrincipalKey(nameof(Role.Id))
                    .OnDelete(DeleteBehavior.Cascade),
                left => left   // lado User
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")          // <<--- nombre real en BD
                    .HasPrincipalKey(nameof(User.Id))
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("UserRoles");
                    join.HasKey("UserId", "RoleId");
                });

            // Backing field para la navegación Roles
            var nav = b.Metadata.FindNavigation(nameof(User.Roles));
            if (nav is not null)
            {
                nav.SetField("_roles");
                nav.SetPropertyAccessMode(PropertyAccessMode.Field);
            }
        }
    }
}
