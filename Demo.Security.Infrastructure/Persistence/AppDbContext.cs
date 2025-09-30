using Demo.Security.Domain.Security;
using Demo.Security.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Demo.Security.Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        public DbSet<PasswordReset> PasswordResets => Set<PasswordReset>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
