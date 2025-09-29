
using Demo.Security.Application.DTOs;
using Demo.Security.Application.Result;
using Demo.Security.Application.Users.Commands;
using Demo.Security.Domain.Users;
using Demo.Security.Infrastructure.Abstractions;

namespace Demo.Security.Application.Users.Handlers
{
    public sealed class LoginUserHandler
    {
        private readonly Infrastructure.Persistence.Repositories.UserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenGenerator _jwt;

        public LoginUserHandler(Infrastructure.Persistence.Repositories.UserRepository users,
                                IPasswordHasher hasher,
                                IJwtTokenGenerator jwt)
        {
            _users = users; _hasher = hasher; _jwt = jwt;
        }

        public async Task<Result<AuthResultDto>> Handle(LoginUserCommand cmd, CancellationToken ct = default)
        {
            var email = Email.Create(cmd.Email);
            var user = await _users.FindByEmailAsync(email, ct);
            if (user is null || !user.IsActive) return Result<AuthResultDto>.Failure(Error.Unauthorized("Credenciales inválidas."));

            if (!_hasher.Verify(user.PasswordHash, cmd.Password))
                return Result<AuthResultDto>.Failure(Error.Unauthorized("Credenciales inválidas."));

            var (token, exp) = _jwt.Generate(user);
            var dto = new AuthResultDto(token, exp, user.UserName, user.Email.Value, user.Roles.Select(r => r.Name).ToArray());
            return Result<AuthResultDto>.Success(dto);
        }
    }
}
