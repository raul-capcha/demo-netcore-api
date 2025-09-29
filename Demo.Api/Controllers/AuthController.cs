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

        public AuthController(LoginUserHandler login) => _login = login;

        public sealed record LoginRequest(string Email, string Password);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var result = await _login.Handle(new LoginUserCommand(req.Email, req.Password), ct);
            return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { error = result.Error!.Message });
        }
    }
}
