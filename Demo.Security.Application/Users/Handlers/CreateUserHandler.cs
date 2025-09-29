using Demo.Security.Application.Result;
using Demo.Security.Application.Users.Commands;
using Demo.Security.Domain.Abstractions;
using Demo.Security.Domain.Users;
using Demo.Security.Infrastructure.Abstractions;

namespace Demo.Security.Application.Users.Handlers
{
    public sealed class CreateUserHandler
    {
        private readonly IRepository<User, Guid> _users;
        private readonly IRepository<Role, Guid> _roles;
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _uow;

        public CreateUserHandler(IRepository<User, Guid> users,
                                 IRepository<Role, Guid> roles,
                                 IPasswordHasher hasher,
                                 IUnitOfWork uow)
        {
            _users = users; _roles = roles; _hasher = hasher; _uow = uow;
        }

        public async Task<Result<Guid>> Handle(CreateUserCommand cmd, CancellationToken ct = default)
        {
            var email = Email.Create(cmd.Email);

            // (Infra debe proveer método para buscar por email; simplificamos aquí con un repositorio concreto en Infra)
            // Para mantener la abstracción, el repositorio concreto proveerá FindByEmailAsync mediante 'as' (ver infra).
            var repo = _users as Infrastructure.Persistence.Repositories.UserRepository;
            var existing = repo is null ? null : await repo.FindByEmailAsync(email, ct);
            if (existing is not null) return Result<Guid>.Failure(Error.Conflict("El email ya existe."));

            var user = User.Create(email, cmd.UserName, _hasher.Hash(cmd.Password));

            // asignar roles
            foreach (var roleName in cmd.Roles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var r = await (repo?.FindRoleByNameAsync(roleName, ct) ?? Task.FromResult<Role?>(null));
                if (r is not null) user.AddRole(r);
            }

            await _users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);
            return Result<Guid>.Success(user.Id);
        }
    }
}
