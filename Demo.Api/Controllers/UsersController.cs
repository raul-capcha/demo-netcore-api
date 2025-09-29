using Demo.Security.Application.DTOs;
using Demo.Security.Application.Users.Commands;
using Demo.Security.Application.Users.Handlers;
using Demo.Security.Application.Users.Mappers;
using Demo.Security.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public sealed class UsersController : ControllerBase
    {
        private readonly CreateUserHandler _create;
        private readonly UserRepository _usersRepo;

        public UsersController(CreateUserHandler create, UserRepository usersRepo)
        {
            _create = create; _usersRepo = usersRepo;
        }

        public sealed record CreateUserRequest(string Email, string UserName, string Password, string[] Roles);

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req, CancellationToken ct)
        {
            var result = await _create.Handle(new CreateUserCommand(req.Email, req.UserName, req.Password, req.Roles), ct);
            return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new { id = result.Value })
                                    : Conflict(new { error = result.Error!.Message });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var u = await _usersRepo.GetByIdAsync(id, ct);
            if (u is null) return NotFound();
            return Ok(UserMapper.ToDto(u));
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var (items, total) = await _usersRepo.ListAsync(Math.Max(1, page), Math.Clamp(pageSize, 1, 100), ct);
            return Ok(new
            {
                page,
                pageSize,
                total,
                data = items.Select(UserMapper.ToDto)
            });
        }
    }
}
