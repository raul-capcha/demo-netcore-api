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
                nb.Property(e => e.Value).HasColumnName("Email").HasMaxLength(256).IsRequired();
                nb.HasIndex(e => e.Value).IsUnique();
            });

            b.Property(x => x.UserName).HasMaxLength(100).IsRequired();
            b.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt);

            // Many-to-many Users <-> Roles
            b.HasMany<Role>("_roles").WithMany()
             .UsingEntity(j =>
                 j.ToTable("UserRoles")
                  .HasData() // no seed aquí; solo definimos tabla
             );
        }
    }
}
