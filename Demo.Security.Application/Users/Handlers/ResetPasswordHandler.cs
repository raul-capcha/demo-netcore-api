using System.Text;
using System.Security.Cryptography;
using Demo.Security.Application.Result;
using Demo.Security.Application.Users.Commands;
using Demo.Security.Infrastructure.Abstractions;

namespace Demo.Security.Application.Users.Handlers
{
    public sealed class ResetPasswordHandler
    {
        private readonly Infrastructure.Persistence.Repositories.UserRepository _users;
        private readonly Infrastructure.Persistence.Repositories.PasswordResetRepository _resets;
        private readonly IPasswordHasher _hasher;

        public ResetPasswordHandler(
            Infrastructure.Persistence.Repositories.UserRepository users,
            Infrastructure.Persistence.Repositories.PasswordResetRepository resets,
            IPasswordHasher hasher)
        {
            _users = users; _resets = resets; _hasher = hasher;
        }

        public async Task<Result<bool>> Handle(ResetPasswordCommand cmd, CancellationToken ct = default)
        {
            var email = Domain.Users.Email.Create(cmd.Email);
            var user = await _users.FindByEmailAsync(email, ct);
            if (user is null || !user.IsActive)
                return Result<bool>.Failure(Error.Unauthorized("Token inválido."));

            var policy = PasswordPolicy.Validate(cmd.NewPassword);
            if (policy is not null) return Result<bool>.Failure(Error.Validation(policy));

            var tokenHash = Sha256Base64(cmd.Token);
            var reset = await _resets.FindValidAsync(user.Id, tokenHash, ct);
            if (reset is null) return Result<bool>.Failure(Error.Unauthorized("Token inválido o expirado."));

            // Cambiar contraseña, invalidar token
            user.SetPasswordHash(_hasher.Hash(cmd.NewPassword));
            reset.MarkUsed();

            await _resets.SaveAsync(ct);
            return Result<bool>.Success(true);
        }

        private static string Sha256Base64(string input)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}
