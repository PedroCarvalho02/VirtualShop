// Controllers/UserController.cs

using LojaVirtualAPI.Data;
using LojaVirtualAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LojaVirtualAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _contexto;
        private readonly IConfiguration _config;

        public UserController(ApplicationDbContext contexto, IConfiguration config)
        {
            _contexto = contexto;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registrar(User usuario)
        {
            if (_contexto.Users == null)
            {
                return Problem("O conjunto de entidades 'ApplicationDbContext.Users' é nulo.");
            }

            // Validar o modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar se o CPF já existe
            if (await _contexto.Users.AnyAsync(u => u.CPF == usuario.CPF))
            {
                return BadRequest("CPF já está em uso.");
            }

            // Verificar se o Email já existe
            if (await _contexto.Users.AnyAsync(u => u.Email == usuario.Email))
            {
                return BadRequest("Email já está em uso.");
            }

            // Hash da senha
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            _contexto.Users.Add(usuario);
            await _contexto.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            if (_contexto.Users == null)
            {
                return Problem("O conjunto de entidades 'ApplicationDbContext.Users' é nulo.");
            }

            var usuario = _contexto.Users.SingleOrDefault(u => u.Email == login.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(login.Password, usuario.Password))
            {
                return Unauthorized("Email ou senha inválidos.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var chaveJwt = _config["Jwt:Key"];
            var expiresInMinutesConfig = _config["Jwt:ExpiresInMinutes"];
            if (string.IsNullOrEmpty(expiresInMinutesConfig))
            {
                return Problem("Configuração 'Jwt:ExpiresInMinutes' não encontrada.");
            }
            if (!double.TryParse(expiresInMinutesConfig, out double expiresInMinutes))
            {
                return Problem("A configuração 'Jwt:ExpiresInMinutes' é inválida ou não está definida.");
            }
            if (string.IsNullOrEmpty(chaveJwt))
            {
                return Problem("Configuração 'Jwt:Key' não encontrada.");
            }
            var chave = Encoding.UTF8.GetBytes(chaveJwt);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, usuario.IsAdmin ? "Admin" : "User")
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
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
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            var usuario = _contexto.Users?.Find(userId);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }
    }
}