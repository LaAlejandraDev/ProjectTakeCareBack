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
    public class ChatMensajesController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public ChatMensajesController(TakeCareContext context)
        {
            _context = context;
        }

        // GET: api/ChatMensajes/chat/5
        [HttpGet("chat/{chatId}")]
        public async Task<ActionResult<IEnumerable<ChatMensaje>>> GetMensajesByChatId(int chatId)
        {
            var mensajes = await _context.ChatMensajes
                .Where(m => m.IdChat == chatId)
                .OrderBy(m => m.Fecha)
                .ToListAsync();

            if (mensajes == null || mensajes.Count == 0)
            {
                return NotFound($"No se encontraron mensajes para el chat con ID {chatId}");
            }

            return Ok(mensajes);
        }


        // GET: api/ChatMensajes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatMensaje>>> GetChatMensajes()
        {
            return await _context.ChatMensajes.ToListAsync();
        }

        // GET: api/ChatMensajes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChatMensaje>> GetChatMensaje(int id)
        {
            var chatMensaje = await _context.ChatMensajes.FindAsync(id);

            if (chatMensaje == null)
            {
                return NotFound();
            }

            return chatMensaje;
        }

        // PUT: api/ChatMensajes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatMensaje(int id, ChatMensaje chatMensaje)
        {
            if (id != chatMensaje.Id)
            {
                return BadRequest();
            }

            _context.Entry(chatMensaje).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatMensajeExists(id))
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

        // POST: api/ChatMensajes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChatMensaje>> PostChatMensaje(ChatMensaje chatMensaje)
        {
            _context.ChatMensajes.Add(chatMensaje);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChatMensaje", new { id = chatMensaje.Id }, chatMensaje);
        }

        // DELETE: api/ChatMensajes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatMensaje(int id)
        {
            var chatMensaje = await _context.ChatMensajes.FindAsync(id);
            if (chatMensaje == null)
            {
                return NotFound();
            }

            _context.ChatMensajes.Remove(chatMensaje);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatMensajeExists(int id)
        {
            return _context.ChatMensajes.Any(e => e.Id == id);
        }
    }
}
