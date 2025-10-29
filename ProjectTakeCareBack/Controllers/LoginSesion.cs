using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectTakeCareBack.Data;
using ProjectTakeCareBack.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectTakeCareBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginSesion : ControllerBase
    {
        private readonly TakeCareContext _context;
        private readonly IConfiguration _configuration;

        public LoginSesion(TakeCareContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UsuarioDTO login)
        {
            var usuario = await _context.Usuarios
               .FirstOrDefaultAsync(u => u.Correo == login.Correo);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });

            bool passwordValida = BCrypt.Net.BCrypt.Verify(login.Contrasena, usuario.Contrasena);

            if (!passwordValida)
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });

            usuario.UltimoAcceso = DateTime.Now;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Rol,
                    UltimoAcceso = usuario.UltimoAcceso
                }
            });
        }
    }
}
