using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTakeCareBack.Data;
using ProjectTakeCareBack.Enums;
using ProjectTakeCareBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTakeCareBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PsicologosController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public PsicologosController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/Psicologoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Psicologo>>> GetPsicologos()
        {
            return await _context.Psicologos.ToListAsync();
        }

        // GET: api/Psicologoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Psicologo>> GetPsicologo(int id)
        {
            var psicologo = await _context.Psicologos.FindAsync(id);

            if (psicologo == null)
            {
                return NotFound();
            }

            return psicologo;
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<object>> GetPsicologoByUsuario(int idUsuario)
        {
            var psicologo = await _context.Psicologos
                .Include(p => p.Usuario) // Incluye la relación con Usuario
                .FirstOrDefaultAsync(p => p.IdUsuario == idUsuario);

            if (psicologo == null)
            {
                return NotFound(new { mensaje = "No se encontró ningún psicólogo asociado a este usuario." });
            }

            // Devuelve el paciente con su información de usuario asociada
            return Ok(new
            {
                psicologo.Id,
                psicologo.CedulaProfesional,
                psicologo.Especialidad,
                psicologo.Descripcion,
                psicologo.ExperienciaAnios,
                psicologo.UniversidadEgreso,
                psicologo.DireccionConsultorio,
                psicologo.CalificacionPromedio,
                psicologo.TotalResenas,
                psicologo.IdUsuario,
                Usuario = new
                {
                    psicologo.Usuario.Id,
                    psicologo.Usuario.Nombre,
                    psicologo.Usuario.ApellidoPaterno,
                    psicologo.Usuario.ApellidoMaterno,
                    psicologo.Usuario.Correo,
                    psicologo.Usuario.Genero,
                    psicologo.Usuario.Telefono,
                    psicologo.Usuario.Rol,
                    psicologo.Usuario.Activo
                }
            });
        }

        // PUT: api/Psicologoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPsicologo(int id, Psicologo psicologo)
        {
            if (id != psicologo.Id)
                return BadRequest("El ID del psicólogo no coincide.");

            var psicologoExistente = await _context.Psicologos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (psicologoExistente == null)
                return NotFound("Psicólogo no encontrado.");

            // Actualización condicional de campos de Psicologo
            if (!string.IsNullOrEmpty(psicologo.CedulaProfesional))
                psicologoExistente.CedulaProfesional = psicologo.CedulaProfesional;

            if (!string.IsNullOrEmpty(psicologo.Especialidad))
                psicologoExistente.Especialidad = psicologo.Especialidad;

            if (!string.IsNullOrEmpty(psicologo.Descripcion))
                psicologoExistente.Descripcion = psicologo.Descripcion;

            if (psicologo.ExperienciaAnios > 0)
                psicologoExistente.ExperienciaAnios = psicologo.ExperienciaAnios;

            if (!string.IsNullOrEmpty(psicologo.UniversidadEgreso))
                psicologoExistente.UniversidadEgreso = psicologo.UniversidadEgreso;

            if (!string.IsNullOrEmpty(psicologo.DireccionConsultorio))
                psicologoExistente.DireccionConsultorio = psicologo.DireccionConsultorio;

            if (psicologo.CalificacionPromedio > 0)
                psicologoExistente.CalificacionPromedio = psicologo.CalificacionPromedio;

            if (psicologo.TotalResenas > 0)
                psicologoExistente.TotalResenas = psicologo.TotalResenas;

            // Actualización condicional de Usuario
            if (psicologo.Usuario != null && psicologoExistente.Usuario != null)
            {
                if (!string.IsNullOrEmpty(psicologo.Usuario.Nombre))
                    psicologoExistente.Usuario.Nombre = psicologo.Usuario.Nombre;

                if (!string.IsNullOrEmpty(psicologo.Usuario.ApellidoPaterno))
                    psicologoExistente.Usuario.ApellidoPaterno = psicologo.Usuario.ApellidoPaterno;

                if (!string.IsNullOrEmpty(psicologo.Usuario.ApellidoMaterno))
                    psicologoExistente.Usuario.ApellidoMaterno = psicologo.Usuario.ApellidoMaterno;

                if (!string.IsNullOrEmpty(psicologo.Usuario.Correo))
                    psicologoExistente.Usuario.Correo = psicologo.Usuario.Correo;

                if (!string.IsNullOrEmpty(psicologo.Usuario.Telefono))
                    psicologoExistente.Usuario.Telefono = psicologo.Usuario.Telefono;

                if (!string.IsNullOrEmpty(psicologo.Usuario.Contrasena))
                    psicologoExistente.Usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(psicologo.Usuario.Contrasena);

                psicologoExistente.Usuario.UltimoAcceso = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Psicólogo actualizado correctamente." });
        }


        // POST: api/Psicologoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Psicologo>> PostPsicologo(Psicologo psicologo)
        {
            if(psicologo.Usuario == null)
            {
                return BadRequest("El objeto es requerido.");
            }

            var usuario = psicologo.Usuario;
            usuario.Rol = ProjectTakeCareBack.Enums.RolUsuario.Psicologo;
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            usuario.FechaRegistro = DateTime.UtcNow;
            usuario.Activo = true;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            psicologo.IdUsuario = usuario.Id;
            psicologo.Usuario = null;

            _context.Psicologos.Add(psicologo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPsicologo", new { id = psicologo.Id }, psicologo);
        }

        // DELETE: api/Psicologoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePsicologo(int id)
        {
            var psicologo = await _context.Psicologos
                .Include(p=> p.Usuario)
                .FirstOrDefaultAsync(p=> p.Id == id);


            if (psicologo == null)
            {
                return NotFound("Psicólogo no encontrado");
            }

            psicologo.Usuario.Activo = false;
            //_context.Psicologos.Remove(psicologo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Psicologos/postUsuarioPsicologo
        [HttpPost("postUsuarioPsicologo")]
        public async Task<ActionResult<Psicologo>> PostPsicologoSimple([FromBody] Psicologo psicologo)
        {
            if (psicologo.IdUsuario == 0)
            {
                return BadRequest(new { mensaje = "El IdUsuario es requerido para crear un psicólogo." });
            }

            var nuevoPsicologo = new Psicologo
            {
                IdUsuario = psicologo.IdUsuario,
                CedulaProfesional = psicologo.CedulaProfesional ?? "",
                Especialidad = psicologo.Especialidad ?? "",
                Descripcion = psicologo.Descripcion ?? "",
                ExperienciaAnios = psicologo.ExperienciaAnios,
                UniversidadEgreso = psicologo.UniversidadEgreso ?? "",
                DireccionConsultorio = psicologo.DireccionConsultorio ?? "",
                CalificacionPromedio = psicologo.CalificacionPromedio,
                TotalResenas = psicologo.TotalResenas,

                Plan = psicologo.Plan,
                Estatus = psicologo.Estatus
            };



            _context.Psicologos.Add(nuevoPsicologo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPsicologo", new { id = nuevoPsicologo.Id }, nuevoPsicologo);
        }

        //Obtenemos la lista de los psicólogos que están pendientes y que necesitan aprobación del admin
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes()
        {
            var pendientes = await _context.Psicologos
                .Where(p => p.Estatus == EstatusAprobacion.Pendiente)
                .Include(p => p.Usuario)
                .ToListAsync();

            return Ok(pendientes);
        }

        //El admin aprueba la suscripción del psicólogo
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> AprobarPsicologo(int id)
        {
            var psicologo = await _context.Psicologos.FindAsync(id);

            if (psicologo == null)
                return NotFound();

            psicologo.Estatus = EstatusAprobacion.Aprobado;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Psicólogo aprobado." });
        }

        //El admin rechaza la suscrippción del psicólogo
        [HttpPut("rechazar/{id}")]
        public async Task<IActionResult> RechazarPsicologo(int id)
        {
            var psicologo = await _context.Psicologos.FindAsync(id);

            if (psicologo == null)
                return NotFound();

            psicologo.Estatus = EstatusAprobacion.Rechazado;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Psicólogo rechazado." });
        }

        //Permite cambiar el plan de suscripción de un psicólogo que ya existe
        [HttpPut("cambiarPlan/{id}")]
        public async Task<IActionResult> CambiarPlan(int id, [FromBody] PlanSuscripcion plan)
        {
            var psicologo = await _context.Psicologos.FindAsync(id);

            if (psicologo == null)
                return NotFound();

            psicologo.Plan = plan;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Plan actualizado." });
        }

        //Devuelve el estatus y el plan de psicólogi basado en el idUsuario
        [HttpGet("estado/{idUsuario}")]
        public async Task<IActionResult> Estado(int idUsuario)
        {
            var psicologo = await _context.Psicologos
                .Where(p => p.IdUsuario == idUsuario)
                .FirstOrDefaultAsync();

            if (psicologo == null)
                return NotFound("Psicólogo no encontrado.");

            return Ok(new
            {
                estatus = psicologo.Estatus,
                plan = psicologo.Plan
            });
        }

        //Permite que un psicólogo solicite una suscripción a un plan
        [HttpPost("{idPsicologo}/suscribirse")]
        public async Task<IActionResult> Suscribirse(int idPsicologo, [FromBody] PlanSuscripcion plan)
        {
            var psicologo = await _context.Psicologos.FindAsync(idPsicologo);

            if (psicologo == null)
                return NotFound("Psicólogo no encontrado.");

            psicologo.Plan = plan;
            psicologo.Estatus = EstatusAprobacion.Pendiente; // requiere aprobación

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Plan solicitado. En espera de aprobación." });
        }





        private bool PsicologoExists(int id)
        {
            return _context.Psicologos.Any(e => e.Id == id);
        }
    }
}
