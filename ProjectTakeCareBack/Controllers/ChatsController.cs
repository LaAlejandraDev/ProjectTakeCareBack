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
    public class ChatsController : ControllerBase
    {
        private readonly TakeCareContext _context;

        public ChatsController(TakeCareContext context)
        {
            _context = context;
        }

        [HttpGet("chatinfo/{id}")]
        public async Task<ActionResult<ChatDTO>> GetChatById(int id)
        {
            var chat = await _context.Chats
                .Include(c => c.Psicologo)
                    .ThenInclude(p => p.Usuario)
                .Include(c => c.Paciente)
                    .ThenInclude(p => p.Usuario)
                .Where(c => c.Id == id)
                .Select(c => new ChatDTO
                {
                    Id = c.Id,
                    IdPsicologo = c.Psicologo.Usuario.Id,
                    IdPaciente = c.Paciente.Usuario.Id,
                    CreadoEn = c.CreadoEn,
                    UltimoMensajeEn = c.UltimoMensajeEn,

                    NombrePsicologo = c.Psicologo.Usuario.Nombre,
                    Especialidad = c.Psicologo.Especialidad,
                    UniversidadEgreso = c.Psicologo.UniversidadEgreso,

                    NombrePaciente = c.Paciente.Usuario.Nombre,
                    ApellidosPaciente = c.Paciente.Usuario.ApellidoMaterno
                })
                .FirstOrDefaultAsync();

            if (chat == null)
            {
                return NotFound($"No se encontró ningún chat con el ID {id}.");
            }

            return Ok(chat);
        }


        [HttpGet("lista")]
        public async Task<ActionResult<IEnumerable<ChatDTO>>> GetChats(
        [FromQuery] int? idPsicologo = null,
        [FromQuery] int? idPaciente = null)
        {
            if (idPsicologo == null && idPaciente == null)
            {
                return BadRequest("Debes proporcionar el id del psicólogo o del paciente.");
            }

            var query = _context.Chats
                .Include(c => c.Psicologo)
                    .ThenInclude(p => p.Usuario)
                .Include(c => c.Paciente)
                    .ThenInclude(p => p.Usuario)
                .AsQueryable();

            if (idPsicologo != null)
            {
                query = query.Where(c => c.IdPsicologo == idPsicologo);
            }
            else if (idPaciente != null)
            {
                query = query.Where(c => c.IdPaciente == idPaciente);
            }

            var chats = await query
                .OrderByDescending(c => c.UltimoMensajeEn)
                .Select(c => new ChatDTO
                {
                    Id = c.Id,
                    IdPsicologo = c.IdPsicologo,
                    IdPaciente = c.IdPaciente,
                    CreadoEn = c.CreadoEn,
                    UltimoMensajeEn = c.UltimoMensajeEn,

                    NombrePsicologo = c.Psicologo.Usuario.Nombre,
                    Especialidad = c.Psicologo.Especialidad,
                    UniversidadEgreso = c.Psicologo.UniversidadEgreso,

                    NombrePaciente = c.Paciente.Usuario.Nombre,
                    ApellidosPaciente = c.Paciente.Usuario.ApellidoMaterno
                })
                .ToListAsync();

            return Ok(chats);
        }


        // GET: api/Chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            return await _context.Chats.ToListAsync();
        }

        // GET: api/Chats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Chat>> GetChat(int id)
        {

            var chat = await _context.Chats
               .Include(c => c.Psicologo)
               .Include(c => c.Paciente)
               .FirstOrDefaultAsync(c => c.Id == id);

            if (chat == null)
            {
                return NotFound();
            }

            return chat;
        }

        // PUT: api/Chats/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChat(int id, Chat chat)
        {
            if (id != chat.Id)
            {
                return BadRequest();
            }

            _context.Entry(chat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChatExists(id))
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

        // POST: api/Chats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Chat>> PostChat(Chat chat)
        {
            // 1. Validar IDs
            if (chat.IdPsicologo <= 0 || chat.IdPaciente <= 0)
                return BadRequest("IdPsicologo e IdPaciente deben ser válidos.");

            // 2. Verificar existencia del psicólogo
            var psicologoExiste = await _context.Psicologos
                .AnyAsync(p => p.Id == chat.IdPsicologo);
            if (!psicologoExiste)
                return NotFound("El psicólogo no existe.");

            // 3. Verificar existencia del paciente
            var pacienteExiste = await _context.Pacientes
                .AnyAsync(p => p.Id == chat.IdPaciente);
            if (!pacienteExiste)
                return NotFound("El paciente no existe.");

            // 4. Verificar si ya existe un chat entre ambos
            var chatExistente = await _context.Chats
                .FirstOrDefaultAsync(c =>
                    c.IdPsicologo == chat.IdPsicologo &&
                    c.IdPaciente == chat.IdPaciente
                );

            if (chatExistente != null)
            {
                // Opcional: actualizar fecha de último acceso
                chatExistente.UltimoMensajeEn = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Conflict(new
                {
                    message = "Ya existe un chat entre este psicólogo y paciente.",
                    chatId = chatExistente.Id,
                    chat = chatExistente
                });
            }

            // 5. No existe → Crear nuevo chat
            chat.CreadoEn = DateTime.UtcNow;

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChat", new { id = chat.Id }, chat);
        }

        // DELETE: api/Chats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChatExists(int id)
        {
            return _context.Chats.Any(e => e.Id == id);
        }
    }
}
