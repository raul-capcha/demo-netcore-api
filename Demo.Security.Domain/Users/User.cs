using Demo.Security.Domain.Abstractions;
using Demo.Security.Domain.Shared;

namespace Demo.Security.Domain.Users
{
    public sealed class User : IEntity<Guid>
    {
        public Guid Id { get; private set; }
        public Email Email { get; private set; } = null!;
        public string UserName { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // ✅ Backing field para EF
        private readonly List<Role> _roles = new();
        public IReadOnlyCollection<Role> Roles => _roles; // <- no AsReadOnly(), EF escribirá en _roles

        private User() { } // EF

        private User(Guid id, Email email, string userName, string passwordHash)
        {
            Id = id;
            Email = email;
            UserName = userName;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
        }

        public static User Create(Email email, string userName, string passwordHash)
        {
            Guard.AgainstNullOrEmpty(userName, nameof(userName));
            Guard.AgainstNullOrEmpty(passwordHash, nameof(passwordHash));
            return new User(Guid.NewGuid(), email, userName.Trim(), passwordHash);
        }

        public void SetPasswordHash(string passwordHash)
        {
            Guard.AgainstNullOrEmpty(passwordHash, nameof(passwordHash));
            PasswordHash = passwordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
        public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }

        public void AddRole(Role role)
        {
            if (!_roles.Any(r => r.Id == role.Id))
                _roles.Add(role);
        }
    }
}
