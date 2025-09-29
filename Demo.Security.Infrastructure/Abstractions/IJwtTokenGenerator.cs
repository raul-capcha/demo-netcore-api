using Demo.Security.Domain.Users;

namespace Demo.Security.Infrastructure.Abstractions
{
    public interface IJwtTokenGenerator
    {
        (string AccessToken, DateTime ExpiresAt) Generate(User user);
    }
}
