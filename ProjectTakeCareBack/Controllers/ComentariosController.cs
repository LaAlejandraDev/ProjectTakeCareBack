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
    public class ComentariosController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public ComentariosController(TakeCareContext context)
        {
            _context = context;
        }

        [HttpGet("Post/{postId}")]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentariosByPost(int postId)
        {
            var comentarios = await _context.Comentarios
                .Where(c => c.IdPost == postId)
                .Include(c => c.Usuario)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            if (comentarios == null || comentarios.Count == 0)
            {
                return NotFound($"No se encontraron comentarios para el post con ID {postId}.");
            }

            return Ok(comentarios);
        }


        // GET: api/Comentarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentarios()
        {
            return await _context.Comentarios.ToListAsync();
        }

        // GET: api/Comentarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comentario>> GetComentario(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound();
            }

            return comentario;
        }

        // PUT: api/Comentarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComentario(int id, Comentario comentario)
        {
            if (id != comentario.Id)
            {
                return BadRequest();
            }

            _context.Entry(comentario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComentarioExists(id))
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

        // DELETE: api/Comentarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }

            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] Comentario comentario)
        {
            if (comentario == null || comentario.IdPost == 0)
            {
                return BadRequest(new { mensaje = "El comentario o el Id del post no es válido." });
            }

            var post = await _context.Posts.FindAsync(comentario.IdPost);
            if (post == null)
            {
                return NotFound(new { mensaje = "El post al que intentas comentar no existe." });
            }

            comentario.Fecha = DateTime.UtcNow;
            _context.Comentarios.Add(comentario);

            post.CommentCount += 1;
            _context.Entry(post).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Comentario agregado exitosamente.",
                comentario = new
                {
                    comentario.Id,
                    comentario.Contenido,
                    comentario.Fecha,
                    comentario.Likes,
                    comentario.Anonimo,
                    comentario.IdUsuario,
                    comentario.IdPost
                },
                totalComentarios = post.CommentCount
            });
        }


        private bool ComentarioExists(int id)
        {
            return _context.Comentarios.Any(e => e.Id == id);
        }
    }
}
