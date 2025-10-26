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
    public class PsicologoesController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public PsicologoesController(TakeCareContext context)
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

        // PUT: api/Psicologoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPsicologo(int id, Psicologo psicologo)
        {
            if (id != psicologo.Id)
            {
                return BadRequest();
            }

            _context.Entry(psicologo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PsicologoExists(id))
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

        // POST: api/Psicologoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Psicologo>> PostPsicologo(Psicologo psicologo)
        {
            _context.Psicologos.Add(psicologo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPsicologo", new { id = psicologo.Id }, psicologo);
        }

        // DELETE: api/Psicologoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePsicologo(int id)
        {
            var psicologo = await _context.Psicologos.FindAsync(id);
            if (psicologo == null)
            {
                return NotFound();
            }

            _context.Psicologos.Remove(psicologo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PsicologoExists(int id)
        {
            return _context.Psicologos.Any(e => e.Id == id);
        }
    }
}
