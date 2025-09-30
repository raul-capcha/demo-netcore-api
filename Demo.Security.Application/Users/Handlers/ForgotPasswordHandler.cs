using System.Text;
using Demo.Security.Application.Result;
using Demo.Security.Application.Users.Commands;
using Demo.Security.Domain.Security;
using Demo.Security.Infrastructure.Abstractions;
using System.Security.Cryptography;
using Demo.Security.Domain.Users;


namespace Demo.Security.Application.Users.Handlers
{
    public sealed class ForgotPasswordHandler
    {
        private readonly Infrastructure.Persistence.Repositories.UserRepository _users;
        private readonly Infrastructure.Persistence.Repositories.PasswordResetRepository _resets;
        private readonly IEmailSender _email;

        public ForgotPasswordHandler(
            Infrastructure.Persistence.Repositories.UserRepository users,
            Infrastructure.Persistence.Repositories.PasswordResetRepository resets,
            IEmailSender email)
        {
            _users = users; _resets = resets; _email = email;
        }

        public async Task<Result<bool>> Handle(ForgotPasswordCommand cmd, string resetBaseUrl, TimeSpan ttl, CancellationToken ct = default)
        {
            var email = Email.Create(cmd.Email);
            var user = await _users.FindByEmailAsync(email, ct);

            // No revelamos existencia del usuario (si no existe, devolvemos éxito igualmente)
            if (user is null) return Result<bool>.Success(true);

            // Generar token aleatorio (32 bytes) y su hash SHA256
            var rawToken = GenerateUrlSafeToken(32);
            var tokenHash = Sha256Base64(rawToken);

            // Registrar token
            var pr = PasswordReset.Create(user.Id, tokenHash, ttl);
            await _resets.AddAsync(pr, ct);
            await _resets.SaveAsync(ct);

            // Construir link de reseteo (ej. https://app/reset?email=...&token=...)
            var url = $"{resetBaseUrl}?email={Uri.EscapeDataString(user.Email.Value)}&token={Uri.EscapeDataString(rawToken)}";

            // Enviar correo (HTML simple)
            var body = $@"<p>Hola {user.UserName},</p>
<p>Para restablecer tu contraseña, haz clic en el siguiente enlace (válido por {ttl.TotalMinutes:N0} minutos):</p>
<p><a href=""{url}"">{url}</a></p>
<p>Si no solicitaste este cambio, ignora este mensaje.</p>";
            await _email.SendAsync(user.Email.Value, "Restablecer contraseña", body, ct);

            return Result<bool>.Success(true);
        }

        private static string GenerateUrlSafeToken(int bytes)
        {
            var data = RandomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(data).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        private static string Sha256Base64(string input)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}
