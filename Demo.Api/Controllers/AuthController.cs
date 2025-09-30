using Demo.Security.Application.Users.Commands;
using Demo.Security.Application.Users.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly LoginUserHandler _login;
        private readonly ForgotPasswordHandler _forgot;
        private readonly ResetPasswordHandler _reset;
        private readonly IConfiguration _cfg;

        public AuthController(LoginUserHandler login, ForgotPasswordHandler forgot, ResetPasswordHandler reset, IConfiguration cfg)
        {
            _login = login; _forgot = forgot; _reset = reset; _cfg = cfg;
        }

        public sealed record LoginRequest(string Email, string Password);
        public sealed record ForgotPasswordRequest(string Email);
        public sealed record ResetPasswordRequest(string Email, string Token, string NewPassword);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var result = await _login.Handle(new LoginUserCommand(req.Email, req.Password), ct);
            return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { error = result.Error!.Message });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req, CancellationToken ct)
        {
            // Base URL para link de reseteo (frontend/backoffice)
            var baseUrl = _cfg["ResetPassword:BaseUrl"] ?? "https://frontoffice.local/reset";
            var ttlMinutes = int.TryParse(_cfg["ResetPassword:Minutes"], out var m) ? m : 30;

            var result = await _forgot.Handle(new ForgotPasswordCommand(req.Email), baseUrl, TimeSpan.FromMinutes(ttlMinutes), ct);
            // Siempre 200 para no filtrar usuarios
            return Ok(new { ok = result.IsSuccess });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req, CancellationToken ct)
        {
            var result = await _reset.Handle(new ResetPasswordCommand(req.Email, req.Token, req.NewPassword), ct);
            return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error!.Message });
        }
    }
}
