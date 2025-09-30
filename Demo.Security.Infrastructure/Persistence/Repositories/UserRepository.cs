using Demo.Security.Domain.Abstractions;
using Demo.Security.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Demo.Security.Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository : IRepository<User, Guid>
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.Users
                        .Include(u => u.Roles) // <- antes: Include("_roles")
                        .FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task AddAsync(User entity, CancellationToken ct = default)
            => await _db.Users.AddAsync(entity, ct);

        public async Task<User?> FindByEmailAsync(Domain.Users.Email email, CancellationToken ct = default)
            => await _db.Users
                        .Include(u => u.Roles) // <- antes: Include("_roles")
                        .FirstOrDefaultAsync(x => x.Email.Value == email.Value, ct);

        public async Task<Role?> FindRoleByNameAsync(string name, CancellationToken ct = default)
            => await _db.Roles.FirstOrDefaultAsync(r => r.Name == name, ct);

        public async Task<(IReadOnlyList<User> Items, int Total)> ListAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var q = _db.Users
                       .AsNoTracking()
                       .Include(u => u.Roles) // <- antes: Include("_roles")
                       .OrderBy(x => x.UserName);

            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return (items, total);
        }
    }
}
