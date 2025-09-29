namespace Demo.Security.Application.DTOs
{
    public sealed record AuthResultDto(string AccessToken, DateTime ExpiresAt, string UserName, string Email, string[] Roles);
}
