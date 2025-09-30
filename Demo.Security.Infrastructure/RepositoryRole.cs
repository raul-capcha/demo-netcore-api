using Demo.Security.Domain.Users;
using Demo.Security.Infrastructure.Persistence;
using Demo.Security.Domain.Abstractions;

namespace Demo.Security.Infrastructure
{
    // Repositorio simple para Role reutilizando DbContext
    public sealed class RepositoryRole : IRepository<Role, Guid>
    {
        private readonly AppDbContext _db;
        public RepositoryRole(AppDbContext db) => _db = db;
        public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _db.Roles.FindAsync([id], ct);
        public async Task AddAsync(Role entity, CancellationToken ct = default) =>
            await _db.Roles.AddAsync(entity, ct);
    }
}
