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
    public class DiarioEmocionalsController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public DiarioEmocionalsController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/DiarioEmocionals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiarioEmocional>>> GetDiarioEmocional()
        {
            return await _context.DiarioEmocional.ToListAsync();
        }

        // GET: api/DiarioEmocionals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiarioEmocional>> GetDiarioEmocional(int id)
        {
            var diarioEmocional = await _context.DiarioEmocional.FindAsync(id);

            if (diarioEmocional == null)
            {
                return NotFound();
            }

            return diarioEmocional;
        }

        // PUT: api/DiarioEmocionals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiarioEmocional(int id, DiarioEmocional diarioEmocional)
        {
            if (id != diarioEmocional.Id)
            {
                return BadRequest();
            }

            _context.Entry(diarioEmocional).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiarioEmocionalExists(id))
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

        // POST: api/DiarioEmocionals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DiarioEmocional>> PostDiarioEmocional(DiarioEmocional diarioEmocional)
        {
            _context.DiarioEmocional.Add(diarioEmocional);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiarioEmocional", new { id = diarioEmocional.Id }, diarioEmocional);
        }

        // DELETE: api/DiarioEmocionals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiarioEmocional(int id)
        {
            var diarioEmocional = await _context.DiarioEmocional.FindAsync(id);
            if (diarioEmocional == null)
            {
                return NotFound();
            }

            _context.DiarioEmocional.Remove(diarioEmocional);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiarioEmocionalExists(int id)
        {
            return _context.DiarioEmocional.Any(e => e.Id == id);
        }
    }
}
