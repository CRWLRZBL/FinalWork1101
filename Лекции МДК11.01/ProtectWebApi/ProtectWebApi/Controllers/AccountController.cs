using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProtectWebApi.Data;
using ProtectWebApi.Models;
using ProtectWebApi.Services;

namespace ProtectWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly GameContext _context;

        public AccountController(TokenService tokenService, GameContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Login))
                return BadRequest("There is no login");
            if (string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("There is no password");
            var dbUser = _context.Users.FirstOrDefault(u => u.Login == user.Login);
            if (dbUser is null)
                return NotFound("There is no user");
            if (dbUser.Password != user.Password)
                return BadRequest("Login or password are incorrect");

            return Ok(_tokenService.GenerateToken(user));
        }
    }
}
