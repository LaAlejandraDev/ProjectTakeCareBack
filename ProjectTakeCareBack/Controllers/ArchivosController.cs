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
    public class ArchivosController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public ArchivosController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/Archivoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Archivo>>> GetArchivos()
        {
            return await _context.Archivos.ToListAsync();
        }

        // GET: api/Archivoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Archivo>> GetArchivo(int id)
        {
            var archivo = await _context.Archivos.FindAsync(id);

            if (archivo == null)
            {
                return NotFound();
            }

            return archivo;
        }

        // PUT: api/Archivoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArchivo(int id, Archivo archivo)
        {
            if (id != archivo.Id)
            {
                return BadRequest();
            }

            _context.Entry(archivo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArchivoExists(id))
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

        // POST: api/Archivoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Archivo>> PostArchivo(Archivo archivo)
        {
            _context.Archivos.Add(archivo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArchivo", new { id = archivo.Id }, archivo);
        }

        // DELETE: api/Archivoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArchivo(int id)
        {
            var archivo = await _context.Archivos.FindAsync(id);
            if (archivo == null)
            {
                return NotFound();
            }

            _context.Archivos.Remove(archivo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArchivoExists(int id)
        {
            return _context.Archivos.Any(e => e.Id == id);
        }
    }
}
