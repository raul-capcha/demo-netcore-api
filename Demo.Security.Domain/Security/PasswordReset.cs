using Demo.Security.Domain.Abstractions;

namespace Demo.Security.Domain.Security
{
    public sealed class PasswordReset : IEntity<Guid>
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string TokenHash { get; private set; } = default!;  // SHA256(Base64UrlToken)
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UsedAt { get; private set; }

        private PasswordReset() { }

        private PasswordReset(Guid id, Guid userId, string tokenHash, DateTime expiresAt)
        {
            Id = id;
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
        }

        public static PasswordReset Create(Guid userId, string tokenHash, TimeSpan ttl)
            => new(Guid.NewGuid(), userId, tokenHash, DateTime.UtcNow.Add(ttl));

        public bool IsUsable() => UsedAt is null && DateTime.UtcNow < ExpiresAt;
        public void MarkUsed() => UsedAt = DateTime.UtcNow;
    }
}
