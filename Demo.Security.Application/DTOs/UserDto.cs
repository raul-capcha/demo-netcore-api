namespace Demo.Security.Application.DTOs
{
    public sealed record UserDto(Guid Id, string UserName, string Email, bool IsActive, string[] Roles);
}
