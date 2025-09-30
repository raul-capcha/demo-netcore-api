using Demo.Security.Application.Result;
using Demo.Security.Application.Users.Commands;
using Demo.Security.Infrastructure.Abstractions;

namespace Demo.Security.Application.Users.Handlers
{
    public sealed class ChangePasswordHandler
    {
        private readonly Infrastructure.Persistence.Repositories.UserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _uow;

        public ChangePasswordHandler(Infrastructure.Persistence.Repositories.UserRepository users, IPasswordHasher hasher, IUnitOfWork uow)
        {
            _users = users; _hasher = hasher; _uow = uow;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand cmd, CancellationToken ct = default)
        {
            var user = await _users.GetByIdAsync(cmd.UserId, ct);
            if (user is null || !user.IsActive) return Result<bool>.Failure(Error.NotFound("Usuario no encontrado."));

            if (!_hasher.Verify(user.PasswordHash, cmd.CurrentPassword))
                return Result<bool>.Failure(Error.Unauthorized("Contraseña actual inválida."));

            var policy = PasswordPolicy.Validate(cmd.NewPassword);
            if (policy is not null) return Result<bool>.Failure(Error.Validation(policy));

            user.SetPasswordHash(_hasher.Hash(cmd.NewPassword));
            await _uow.SaveChangesAsync(ct);
            return Result<bool>.Success(true);
        }
    }
}
