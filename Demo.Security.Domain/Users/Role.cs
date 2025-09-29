
using Demo.Security.Domain.Abstractions;

namespace Demo.Security.Domain.Users
{
    public sealed class Role : IEntity<Guid>
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = default!;

        private Role() { }
        public Role(Guid id, string name)
        {
            Id = id;
            Name = name.Trim();
        }
    }
}
