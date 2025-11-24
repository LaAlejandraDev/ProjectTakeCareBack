using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ProjectTakeCareBack.Data;
using ProjectTakeCareBack.Enums;
using ProjectTakeCareBack.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTakeCareBack.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly TakeCareContext _context;
        private readonly IConfiguration _configuration;


        public UsuariosController(TakeCareContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        [HttpGet("info/{id}")]
        public async Task<IActionResult> GetUserInformation(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Psicologo)
                .Include(u => u.Paciente)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            var dto = new UserInformationDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                ApellidoPaterno = usuario.ApellidoPaterno,
                ApellidoMaterno = usuario.ApellidoMaterno,
                Genero = usuario.Genero,
                Correo = usuario.Correo,
                Telefono = usuario.Telefono,
                Rol = usuario.Rol
            };

            if (usuario.Rol == RolUsuario.Psicologo)
            {
                dto.Psicologo = usuario.Psicologo;
            }
            else if (usuario.Rol == RolUsuario.Paciente)
            {
                dto.Paciente = usuario.Paciente;
            }

            return Ok(dto);
        }


        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest(new {mensaje="El ID del usuario no coincide con el parámetro proporcionado."});
            }

            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id); 
            if (usuarioExistente == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            bool correoExistente = await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo && u.Id != id);

            if(correoExistente)
            {
                return BadRequest(new { mensaje = "El correo electrónico ya está en uso por otro usuario." });
            }

            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.ApellidoPaterno = usuario.ApellidoPaterno;
            usuarioExistente.ApellidoMaterno = usuario.ApellidoMaterno;
            usuarioExistente.Correo = usuario.Correo;
            usuarioExistente.Telefono = usuario.Telefono;
            usuarioExistente.Rol = usuario.Rol;
            usuarioExistente.Activo = usuario.Activo;

            if (!string.IsNullOrWhiteSpace(usuario.Contrasena))
            {
                usuarioExistente.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            }


            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Uusaurio actualizado correctamente." });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { mensaje = "Error al actualizar el usuario.", detalle = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Ocurrió un error inesperado.", detalle = ex.Message });
            }
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            try
            {
                bool existeCorreo = await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo);
                if (string.IsNullOrWhiteSpace(usuario.Contrasena))
                {
                    return BadRequest(new {mensaje = "La contraseña es obligatoria. "});
                }

                if (existeCorreo)
                {
                    return BadRequest(new { mensaje = "Ya existe un usuario registrado con ese correo." });
                }

                usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
               

            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al crear el usuario.",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Activo = false;
           // _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();


            return Ok(new { mensaje = "Usuario desactivado correctamente." });
        }

        [HttpPost("registro")]
        public async Task<IActionResult> RegistrarPsicologo([FromBody] RegistroUserDTO modelo)
        {
            var usuario = new Usuario
            {
                Nombre = modelo.Nombre,
                Correo = modelo.Correo,
                Contrasena = BCrypt.Net.BCrypt.HashPassword(modelo.Contrasena),
                Rol = RolUsuario.Psicologo,
                FechaRegistro = DateTime.UtcNow,
                UltimoAcceso = DateTime.UtcNow,
                Activo = true
            };

            usuario.Psicologo = new Psicologo
            {
                Plan = PlanSuscripcion.Gratis,
                Estatus = EstatusAprobacion.Aprobado
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var claims = new[]
            {
                  new Claim("idUsuario", usuario.Id.ToString()),
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

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                mensaje = "Usuario registrado",
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Rol
                },
                idPsicologo = usuario.Psicologo.Id,
                token = tokenString
            });
        }


        //Obtener datos del usuario para el editperfil
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized(new { mensaje = "Token inválido" });

            int userId = int.Parse(userIdClaim);

            var usuario = await _context.Usuarios.FindAsync(userId);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return Ok(new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.ApellidoPaterno,
                usuario.ApellidoMaterno,
                usuario.Genero,
                usuario.Correo,
                usuario.Telefono,
                usuario.Rol,
                usuario.Activo,
                usuario.FechaRegistro,
                usuario.UltimoAcceso,
                usuario.Suscripcion
            });
        }


        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
