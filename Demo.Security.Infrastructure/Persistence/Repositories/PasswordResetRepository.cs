using Demo.Security.Domain.Security;
using Microsoft.EntityFrameworkCore;

namespace Demo.Security.Infrastructure.Persistence.Repositories
{
    public sealed class PasswordResetRepository
    {
        private readonly AppDbContext _db;
        public PasswordResetRepository(AppDbContext db) => _db = db;

        public Task AddAsync(PasswordReset entity, CancellationToken ct = default)
            => _db.PasswordResets.AddAsync(entity, ct).AsTask();

        public async Task<PasswordReset?> FindValidAsync(Guid userId, string tokenHash, CancellationToken ct = default)
            => await _db.PasswordResets
                        .Where(p => p.UserId == userId && p.TokenHash == tokenHash && p.UsedAt == null && p.ExpiresAt > DateTime.UtcNow)
                        .FirstOrDefaultAsync(ct);

        public Task<int> SaveAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}
