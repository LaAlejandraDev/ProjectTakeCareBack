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
    public class ValoracionesController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public ValoracionesController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/Valoraciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Valoracion>>> GetValoracion()
        {
            return await _context.Valoracion.ToListAsync();
        }

        // GET: api/Valoraciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Valoracion>> GetValoracion(int id)
        {
            var valoracion = await _context.Valoracion.FindAsync(id);

            if (valoracion == null)
            {
                return NotFound();
            }

            return valoracion;
        }

        // PUT: api/Valoraciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutValoracion(int id, Valoracion valoracion)
        {
            if (id != valoracion.Id)
            {
                return BadRequest();
            }

            _context.Entry(valoracion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValoracionExists(id))
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

        // POST: api/Valoraciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Valoracion>> PostValoracion(Valoracion valoracion)
        {
            // Verificar que el psicólogo exista
            var psicologo = await _context.Psicologos
                .FirstOrDefaultAsync(p => p.Id == valoracion.IdPsicologo);

            if (psicologo == null)
                return BadRequest("El psicólogo no existe.");

            // Guardar la nueva valoración
            _context.Valoracion.Add(valoracion);
            await _context.SaveChangesAsync();

            // === Recalcular promedio ===

            // Obtener todas las calificaciones del psicólogo
            var valoraciones = await _context.Valoracion
                .Where(v => v.IdPsicologo == valoracion.IdPsicologo)
                .Select(v => v.Calificacion)
                .ToListAsync();

            // Calcular promedio
            float promedio = valoraciones.Average();

            // Actualizar estadísticas del psicólogo
            psicologo.CalificacionPromedio = (decimal)promedio;
            psicologo.TotalResenas = valoraciones.Count;

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetValoracion", new { id = valoracion.Id }, valoracion);
        }

        // DELETE: api/Valoraciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValoracion(int id)
        {
            var valoracion = await _context.Valoracion.FindAsync(id);
            if (valoracion == null)
            {
                return NotFound();
            }

            _context.Valoracion.Remove(valoracion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ValoracionExists(int id)
        {
            return _context.Valoracion.Any(e => e.Id == id);
        }
    }
}
