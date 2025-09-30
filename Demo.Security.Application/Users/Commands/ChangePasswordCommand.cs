namespace Demo.Security.Application.Users.Commands
{
    public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);
}
