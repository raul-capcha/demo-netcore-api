using Demo.Security.Domain.Abstractions;
using Demo.Security.Domain.Users;
using Demo.Security.Infrastructure.Abstractions;
using Demo.Security.Infrastructure.Persistence;
using Demo.Security.Infrastructure.Persistence.Repositories;
using Demo.Security.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Security.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRepository<User, Guid>, UserRepository>();
            services.AddScoped<UserRepository>(); // para métodos específicos
            services.AddScoped<IRepository<Role, Guid>, RepositoryRole>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }

    // Repositorio simple para Role reutilizando DbContext
    internal sealed class RepositoryRole : IRepository<Role, Guid>
    {
        private readonly AppDbContext _db;
        public RepositoryRole(AppDbContext db) => _db = db;
        public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _db.Roles.FindAsync(new object?[] { id }, ct);
        public async Task AddAsync(Role entity, CancellationToken ct = default) =>
            await _db.Roles.AddAsync(entity, ct);
    }
}
