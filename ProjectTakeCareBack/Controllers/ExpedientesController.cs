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
    public class ExpedientesController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public ExpedientesController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/Expedientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expediente>>> GetExpediente()
        {
            return await _context.Expediente.ToListAsync();
        }

        [HttpGet("psicologo/{idPsicologo}")]
        public async Task<ActionResult<IEnumerable<Expediente>>> GetExpedientesByPsicologo(int idPsicologo)
        {
            var expedientes = await _context.Expediente
                .Include(e => e.Paciente)
                    .ThenInclude(p => p.Usuario)
                .Include(e => e.Citas)
                .Include(e => e.Diarios)
                .Where(e => e.Citas.Any(c => c.IdPsicologo == idPsicologo))
                .ToListAsync();

            if (!expedientes.Any())
            {
                return NotFound("No se encontraron expedientes para este psicólogo.");
            }

            return expedientes;
        }

        // GET: api/Expedientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expediente>> GetExpediente(int id)
        {
            var expediente = await _context.Expediente
                .Include(e => e.Paciente)
                    .ThenInclude(p => p.Usuario)
                .Include(e => e.Citas)
                .Include(e => e.Diarios)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expediente == null)
            {
                return NotFound("Expediente no encontrado.");
            }

            return expediente;
        }

        // PUT: api/Expedientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpediente(int id, Expediente expediente)
        {
            if (id != expediente.Id)
            {
                return BadRequest();
            }

            _context.Entry(expediente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpedienteExists(id))
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

        // POST: api/Expedientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Expediente>> PostExpediente(Expediente expediente)
        {
            _context.Expediente.Add(expediente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpediente", new { id = expediente.Id }, expediente);
        }

        // DELETE: api/Expedientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpediente(int id)
        {
            var expediente = await _context.Expediente.FindAsync(id);
            if (expediente == null)
            {
                return NotFound();
            }

            _context.Expediente.Remove(expediente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpedienteExists(int id)
        {
            return _context.Expediente.Any(e => e.Id == id);
        }
    }
}
