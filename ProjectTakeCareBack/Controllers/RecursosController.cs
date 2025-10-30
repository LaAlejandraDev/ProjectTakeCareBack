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
    public class RecursosController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public RecursosController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/Recursos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recurso>>> GetRecursos()
        {
            return await _context.Recursos.ToListAsync();
        }

        // GET: api/Recursos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recurso>> GetRecurso(int id)
        {
            var recurso = await _context.Recursos.FindAsync(id);

            if (recurso == null)
            {
                return NotFound();
            }

            return recurso;
        }

        // PUT: api/Recursos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecurso(int id, Recurso recurso)
        {
            if (id != recurso.Id)
            {
                return BadRequest();
            }

            _context.Entry(recurso).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecursoExists(id))
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

        // POST: api/Recursos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Recurso>> PostRecurso(Recurso recurso)
        {
            _context.Recursos.Add(recurso);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecurso", new { id = recurso.Id }, recurso);
        }

        // DELETE: api/Recursos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecurso(int id)
        {
            var recurso = await _context.Recursos.FindAsync(id);
            if (recurso == null)
            {
                return NotFound();
            }

            _context.Recursos.Remove(recurso);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecursoExists(int id)
        {
            return _context.Recursos.Any(e => e.Id == id);
        }
    }
}
