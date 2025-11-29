using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTakeCareBack.Data;
using ProjectTakeCareBack.Models;

namespace ProjectTakeCareBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public PacientesController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/Pacientes/usuario/5
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<object>> GetPacienteByUsuario(int idUsuario)
        {
            var paciente = await _context.Pacientes
                .Include(p => p.Usuario) // Incluye la relación con Usuario
                .FirstOrDefaultAsync(p => p.IdUsuario == idUsuario);

            if (paciente == null)
            {
                return NotFound(new { mensaje = "No se encontró ningún paciente asociado a este usuario." });
            }

            // Devuelve el paciente con su información de usuario asociada
            return Ok(new
            {
                paciente.Id,
                paciente.Ciudad,
                paciente.EstadoCivil,
                paciente.Diagnostico,
                paciente.AntecedentesMedicos,
                paciente.ContactoEmergencia,
                paciente.IdUsuario,
                Usuario = new
                {
                    paciente.Usuario.Id,
                    paciente.Usuario.Nombre,
                    paciente.Usuario.ApellidoPaterno,
                    paciente.Usuario.ApellidoMaterno,
                    paciente.Usuario.Correo,
                    paciente.Usuario.Genero,
                    paciente.Usuario.Telefono,
                    paciente.Usuario.Rol,
                    paciente.Usuario.Activo
                }
            });
        }


        // GET: api/Pacientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes()
        {
            return await _context.Pacientes.ToListAsync();
        }

        // GET: api/Pacientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Paciente>> GetPaciente(int id)
        {
            var paciente = await _context.Pacientes
                .Include(p => p.Usuario) // Incluye la información del usuario relacionado
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paciente == null)
            {
                return NotFound(new { mensaje = "Paciente no encontrado" });
            }

            return paciente;
        }

        // PUT: api/Pacientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaciente(int id, Paciente paciente)
        {
            if (id != paciente.Id)
            {
                return BadRequest("El ID del paciente no coincide.");
            }

            var pacienteExistente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pacienteExistente == null)
            {
                return NotFound("Paciente no encontrado.");
            }

            pacienteExistente.Ciudad = paciente.Ciudad ?? pacienteExistente.Ciudad;
            pacienteExistente.EstadoCivil = paciente.EstadoCivil ?? pacienteExistente.EstadoCivil;
            pacienteExistente.Diagnostico = paciente.Diagnostico ?? pacienteExistente.Diagnostico;
            pacienteExistente.AntecedentesMedicos = paciente.AntecedentesMedicos ?? pacienteExistente.AntecedentesMedicos;
            pacienteExistente.ContactoEmergencia = paciente.ContactoEmergencia ?? pacienteExistente.ContactoEmergencia;

            if (paciente.Usuario != null && pacienteExistente.Usuario != null)
            {
                pacienteExistente.Usuario.Nombre = paciente.Usuario.Nombre ?? pacienteExistente.Usuario.Nombre;
                pacienteExistente.Usuario.ApellidoPaterno = paciente.Usuario.ApellidoPaterno ?? pacienteExistente.Usuario.ApellidoPaterno;
                pacienteExistente.Usuario.ApellidoMaterno = paciente.Usuario.ApellidoMaterno ?? pacienteExistente.Usuario.ApellidoMaterno;
                pacienteExistente.Usuario.Genero = paciente.Usuario.Genero ?? pacienteExistente.Usuario.Genero;
                pacienteExistente.Usuario.Correo = paciente.Usuario.Correo ?? pacienteExistente.Usuario.Correo;
                pacienteExistente.Usuario.Telefono = paciente.Usuario.Telefono ?? pacienteExistente.Usuario.Telefono;

                if (!string.IsNullOrEmpty(paciente.Usuario.Contrasena))
                {
                    pacienteExistente.Usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(paciente.Usuario.Contrasena);
                }

                pacienteExistente.Usuario.Activo = paciente.Usuario.Activo;
                pacienteExistente.Usuario.UltimoAcceso = DateTime.UtcNow;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PacienteExists(id))
                {
                    return NotFound("El paciente no existe.");
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { mensaje = "Paciente actualizado correctamente." });
        }


        // POST: api/Pacientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Paciente>> PostPaciente(Paciente paciente)
        {
            if (paciente.Usuario == null)
            {
                return BadRequest("El objeto es requerido.");
            }

            // Crear usuario y configurarlo
            var usuario = paciente.Usuario;
            usuario.Rol = ProjectTakeCareBack.Enums.RolUsuario.Paciente;
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            usuario.FechaRegistro = DateTime.UtcNow;
            usuario.Activo = true;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Asociar id del usuario creado al paciente
            paciente.IdUsuario = usuario.Id;
            paciente.Usuario = null;

            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();

            var expediente = new Expediente
            {
                PacienteId = paciente.Id
            };

            _context.Expediente.Add(expediente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaciente", new { id = paciente.Id }, paciente);
        }

        // DELETE: api/Pacientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            var paciente = await _context.Pacientes
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (paciente == null)
            {
                return NotFound("Paciente no encontrado.");
            }

            paciente.Usuario.Activo = false;
            //_context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Pacientes/postUsuarioPaciente
        [HttpPost("postUsuarioPaciente")]
        public async Task<ActionResult<Paciente>> PostPacienteSimple([FromBody] Paciente paciente)
        {
            if (paciente.IdUsuario == 0)
            {
                return BadRequest(new { mensaje = "El IdUsuario es requerido para crear un paciente." });
            }

            var nuevoPaciente = new Paciente
            {
                IdUsuario = paciente.IdUsuario,
                Ciudad = paciente.Ciudad ?? "",
                EstadoCivil = paciente.EstadoCivil ?? "",
                Diagnostico = paciente.Diagnostico ?? "",
                AntecedentesMedicos = paciente.AntecedentesMedicos ?? "",
                ContactoEmergencia = paciente.ContactoEmergencia ?? ""
            };

            _context.Pacientes.Add(nuevoPaciente);
            await _context.SaveChangesAsync();

            var expediente = new Expediente
            {
                PacienteId = nuevoPaciente.Id
            };

            _context.Expediente.Add(expediente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaciente", new { id = nuevoPaciente.Id }, nuevoPaciente);
        }


        private bool PacienteExists(int id)
        {
            return _context.Pacientes.Any(e => e.Id == id);
        }
    }
}