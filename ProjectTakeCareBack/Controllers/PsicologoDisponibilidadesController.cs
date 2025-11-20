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
    public class PsicologoDisponibilidadesController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public PsicologoDisponibilidadesController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/PsicologoDisponibilidades/psicologo/5
        [HttpGet("psicologo/{idPsicologo}")]
        public async Task<ActionResult<IEnumerable<PsicologoDisponibilidad>>>
            GetDisponibilidadPorPsicologo(int idPsicologo)
        {
            var psicologoExiste = await _context.Psicologos.AnyAsync(p => p.Id == idPsicologo);

            if (!psicologoExiste)
                return NotFound("El psicólogo no existe.");

            var disponibilidades = await _context.PsicologoDisponibilidad
                .Where(d => d.IdPsicologo == idPsicologo)
                .OrderBy(d => d.Fecha)
                .ToListAsync();

            if (disponibilidades == null || disponibilidades.Count == 0)
                return NotFound("El psicólogo no tiene días laborales registrados.");

            return disponibilidades;
        }

        // GET: api/PsicologoDisponibilidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PsicologoDisponibilidad>>> GetPsicologoDisponibilidad()
        {
            return await _context.PsicologoDisponibilidad.ToListAsync();
        }

        // GET: api/PsicologoDisponibilidades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PsicologoDisponibilidad>> GetPsicologoDisponibilidad(int id)
        {
            var psicologoDisponibilidad = await _context.PsicologoDisponibilidad.FindAsync(id);

            if (psicologoDisponibilidad == null)
            {
                return NotFound();
            }

            return psicologoDisponibilidad;
        }

        // PUT: api/PsicologoDisponibilidades/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPsicologoDisponibilidad(int id, PsicologoDisponibilidad psicologoDisponibilidad)
        {
            if (id != psicologoDisponibilidad.Id)
            {
                return BadRequest();
            }

            _context.Entry(psicologoDisponibilidad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PsicologoDisponibilidadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PsicologoDisponibilidades
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PsicologoDisponibilidad>> PostPsicologoDisponibilidad([FromBody] CrearDisponibilidadDto dto)
        {
            // Normalizar fecha (sin hora)
            var fecha = dto.Fecha.Date;
            var hoy = DateTime.Today;

            var psicologoExiste = await _context.Psicologos.AnyAsync(p => p.Id == dto.IdPsicologo);
            if (!psicologoExiste)
                return BadRequest("El psicólogo no existe.");

            if (fecha < hoy)
                return BadRequest("No se pueden registrar días anteriores a hoy.");

            var yaExiste = await _context.PsicologoDisponibilidad
                .AnyAsync(d => d.IdPsicologo == dto.IdPsicologo && d.Fecha == fecha);

            if (yaExiste)
                return BadRequest("Este día ya está registrado como día laboral.");

            // CREACIÓN del registro
            var disponibilidad = new PsicologoDisponibilidad
            {
                IdPsicologo = dto.IdPsicologo,
                Fecha = fecha,
                CupoMaximo = 4,
                CitasAgendadas = 0
            };

            _context.PsicologoDisponibilidad.Add(disponibilidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPsicologoDisponibilidad", new { id = disponibilidad.Id }, disponibilidad);
        }

        // DELETE: api/PsicologoDisponibilidades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePsicologoDisponibilidad(int id)
        {
            var psicologoDisponibilidad = await _context.PsicologoDisponibilidad.FindAsync(id);
            if (psicologoDisponibilidad == null)
            {
                return NotFound();
            }

            _context.PsicologoDisponibilidad.Remove(psicologoDisponibilidad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PsicologoDisponibilidadExists(int id)
        {
            return _context.PsicologoDisponibilidad.Any(e => e.Id == id);
        }
    }
}
