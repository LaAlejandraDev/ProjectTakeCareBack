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
    public class CitaComentariosController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public CitaComentariosController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/CitaComentarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CitaComentario>>> GetCitaComentario()
        {
            return await _context.CitaComentario.ToListAsync();
        }

        // GET: api/CitaComentarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CitaComentario>> GetCitaComentario(int id)
        {
            var citaComentario = await _context.CitaComentario.FindAsync(id);

            if (citaComentario == null)
            {
                return NotFound();
            }

            return citaComentario;
        }

        // PUT: api/CitaComentarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCitaComentario(int id, CitaComentario citaComentario)
        {
            if (id != citaComentario.Id)
            {
                return BadRequest();
            }

            _context.Entry(citaComentario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitaComentarioExists(id))
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

        // POST: api/CitaComentarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CitaComentario>> PostCitaComentario(CitaComentario citaComentario)
        {
            _context.CitaComentario.Add(citaComentario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCitaComentario", new { id = citaComentario.Id }, citaComentario);
        }

        // DELETE: api/CitaComentarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCitaComentario(int id)
        {
            var citaComentario = await _context.CitaComentario.FindAsync(id);
            if (citaComentario == null)
            {
                return NotFound();
            }

            _context.CitaComentario.Remove(citaComentario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CitaComentarioExists(int id)
        {
            return _context.CitaComentario.Any(e => e.Id == id);
        }
    }
}
