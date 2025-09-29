namespace Demo.Security.Application.Users.Commands
{
    public sealed record CreateUserCommand(string Email, string UserName, string Password, string[] Roles);
}
