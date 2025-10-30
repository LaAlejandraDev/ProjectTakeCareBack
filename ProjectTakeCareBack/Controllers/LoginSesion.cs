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

        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyToken([FromBody] TokenVerifyRequest request)
        {
            // Validar que el token no venga vacío
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(new { mensaje = "Token no proporcionado." });

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                // Validar el token
                tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Extraer el Id del usuario desde el token
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { mensaje = "Token inválido" });

                int userId = int.Parse(userIdClaim.Value);

                // Verificar que el usuario exista
                var usuario = await _context.Usuarios.FindAsync(userId);
                if (usuario == null)
                    return Unauthorized(new { mensaje = "Usuario no encontrado" });

                // Si quieres, puedes devolver info adicional
                return Ok(new
                {
                    mensaje = "Token válido",
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
            catch (Exception)
            {
                return Unauthorized(new { mensaje = "Token inválido o expirado" });
            }
        }

        // Modelo para la solicitud
        public class TokenVerifyRequest
        {
            public string Token { get; set; } = null!;
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