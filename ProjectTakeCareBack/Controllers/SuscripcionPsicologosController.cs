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
    public class SuscripcionPsicologosController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public SuscripcionPsicologosController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/SuscripcionPsicologoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SuscripcionPsicologo>>> GetSuscripciones()
        {
            return await _context.Suscripciones.ToListAsync();
        }

        // GET: api/SuscripcionPsicologoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SuscripcionPsicologo>> GetSuscripcionPsicologo(int id)
        {
            var suscripcionPsicologo = await _context.Suscripciones.FindAsync(id);

            if (suscripcionPsicologo == null)
            {
                return NotFound();
            }

            return suscripcionPsicologo;
        }

        // PUT: api/SuscripcionPsicologoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuscripcionPsicologo(int id, SuscripcionPsicologo suscripcionPsicologo)
        {
            if (id != suscripcionPsicologo.Id)
            {
                return BadRequest();
            }

            _context.Entry(suscripcionPsicologo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuscripcionPsicologoExists(id))
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

        // POST: api/SuscripcionPsicologoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SuscripcionPsicologo>> PostSuscripcionPsicologo(SuscripcionPsicologo suscripcionPsicologo)
        {
            _context.Suscripciones.Add(suscripcionPsicologo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSuscripcionPsicologo", new { id = suscripcionPsicologo.Id }, suscripcionPsicologo);
        }

        // DELETE: api/SuscripcionPsicologoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuscripcionPsicologo(int id)
        {
            var suscripcionPsicologo = await _context.Suscripciones.FindAsync(id);
            if (suscripcionPsicologo == null)
            {
                return NotFound();
            }

            _context.Suscripciones.Remove(suscripcionPsicologo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SuscripcionPsicologoExists(int id)
        {
            return _context.Suscripciones.Any(e => e.Id == id);
        }
    }
}
