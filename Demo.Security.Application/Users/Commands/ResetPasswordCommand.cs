namespace Demo.Security.Application.Users.Commands
{
    public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword);
}
