using Demo.Security.Application.DTOs;
using Demo.Security.Domain.Users;

namespace Demo.Security.Application.Users.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(User u) =>
            new(u.Id, u.UserName, u.Email.Value, u.IsActive, u.Roles.Select(r => r.Name).ToArray());
    }
}
