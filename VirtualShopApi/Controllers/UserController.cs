using BCrypt.Net;
using LojaVirtualAPI.Data;
using LojaVirtualAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LojaVirtualAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _contexto;

        public UserController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registrar(User usuario)
        {
            if (_contexto.Users == null)
            {
                return Problem("O conjunto de entidades 'ApplicationDbContext.Users' é nulo.");
            }

            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

            _contexto.Users.Add(usuario);
            await _contexto.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            if (_contexto.Users == null)
            {
                return Problem("O conjunto de entidades 'ApplicationDbContext.Users' é nulo.");
            }
            var usuario = _contexto.Users.SingleOrDefault(u => u.Email == login.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(login.Password, usuario.Password))
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var chave = Encoding.UTF8.GetBytes("0kPQ7LB8g7yrgkg5DGs5DU6rKZTQIZ5LCCED22BA9B1529AA3B9BA2B37BBCE");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, usuario.IsAdmin ? "Admin" : "User")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token é necessário.");
            }

            var tokenRevogado = new RevokedToken
            {
                Token = token,
                RevokedAt = DateTime.UtcNow
            };

            if (_contexto.RevokedTokens == null)
            {
                return Problem("O conjunto de entidades 'ApplicationDbContext.RevokedTokens' é nulo.");
            }

            _contexto.RevokedTokens.Add(tokenRevogado);
            await _contexto.SaveChangesAsync();

            return Ok("Logout realizado com sucesso.");
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Perfil()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var usuario = _contexto.Users?.Find(userId);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }
    }
}