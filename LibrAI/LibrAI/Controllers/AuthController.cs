using LibrAI.Shared;
using Microsoft.AspNetCore.Mvc;

namespace LibrAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Geçici kullanıcı listesi (in-memory)
        private static readonly List<UserDto> Users = new();

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDto user)
        {
            if (Users.Any(u => u.Email == user.Email))
                return BadRequest("Bu e-posta zaten kayıtlı.");

            Users.Add(user);
            return Ok("Kayıt başarılı.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto login)
        {
            var user = Users.FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);

            if (user == null)
                return Unauthorized("Geçersiz e-posta veya şifre.");

            return Ok("Giriş başarılı.");
        }
    }
}
