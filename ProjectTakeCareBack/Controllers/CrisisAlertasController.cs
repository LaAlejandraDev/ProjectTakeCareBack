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
    public class CrisisAlertasController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public CrisisAlertasController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/CrisisAlertas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CrisisAlerta>>> GetCrisisAlerts()
        {
            return await _context.CrisisAlerts.ToListAsync();
        }

        // GET: api/CrisisAlertas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CrisisAlerta>> GetCrisisAlerta(int id)
        {
            var crisisAlerta = await _context.CrisisAlerts.FindAsync(id);

            if (crisisAlerta == null)
            {
                return NotFound();
            }

            return crisisAlerta;
        }

        // PUT: api/CrisisAlertas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCrisisAlerta(int id, CrisisAlerta crisisAlerta)
        {
            if (id != crisisAlerta.Id)
            {
                return BadRequest();
            }

            _context.Entry(crisisAlerta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CrisisAlertaExists(id))
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

        // POST: api/CrisisAlertas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CrisisAlerta>> PostCrisisAlerta(CrisisAlerta crisisAlerta)
        {
            _context.CrisisAlerts.Add(crisisAlerta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCrisisAlerta", new { id = crisisAlerta.Id }, crisisAlerta);
        }

        // DELETE: api/CrisisAlertas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrisisAlerta(int id)
        {
            var crisisAlerta = await _context.CrisisAlerts.FindAsync(id);
            if (crisisAlerta == null)
            {
                return NotFound();
            }

            _context.CrisisAlerts.Remove(crisisAlerta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CrisisAlertaExists(int id)
        {
            return _context.CrisisAlerts.Any(e => e.Id == id);
        }
    }
}
