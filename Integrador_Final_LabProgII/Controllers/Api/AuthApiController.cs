using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ITManager.Repositories.Interfaces;
using ITManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ITManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthApiController(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public class LoginApiRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginApiRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email y contraseña son obligatorios." });
            }

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario == null || !_passwordHasher.VerifyPassword(request.Password, usuario.PasswordHash))
            {
                return Unauthorized(new { message = "Credenciales incorrectas." });
            }

            if (!usuario.Activo)
            {
                return Unauthorized(new { message = "Usuario inactivo en el sistema." });
            }

            var jwtKey = _configuration["Jwt:Key"] ?? "ITManager_ClaveSecretaSuperSeguraParaFirmarTokensJwt2026!*";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "ITManagerServer";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "ITManagerClients";
            var durationHours = int.TryParse(_configuration["Jwt:DurationInHours"], out var h) ? h : 8;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}".Trim()),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(durationHours),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                expiration = tokenDescriptor.Expires,
                usuario = new
                {
                    id = usuario.Id,
                    nombre = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                    email = usuario.Email,
                    rol = usuario.Rol.ToString()
                }
            });
        }
    }
}
