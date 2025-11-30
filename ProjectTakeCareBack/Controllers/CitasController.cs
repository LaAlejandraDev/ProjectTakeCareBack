using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjectTakeCareBack.Data;
using ProjectTakeCareBack.Hubs;
using ProjectTakeCareBack.Migrations;
using ProjectTakeCareBack.Models;

namespace ProjectTakeCareBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        private readonly TakeCareContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public CitasController(TakeCareContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Citas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitas()
        {
            return await _context.Citas.ToListAsync();
        }

        // GET: api/Citas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cita>> GetCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }

            return cita;
        }

        // GET: api/Citas/psicologo/1
        [HttpGet("psicologo/{idPsicologo}")]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitasPorPsicologo(int idPsicologo)
        {
            if (idPsicologo <= 0)
                return BadRequest("Id de psicólogo inválido.");

            var citas = await _context.Citas
                .Where(c => c.IdPsicologo == idPsicologo)
                .OrderBy(c => c.FechaInicio)
                .ToListAsync();

            if (citas == null || citas.Count == 0)
                return NotFound("El psicólogo no tiene citas registradas.");

            return Ok(citas);
        }

        // GET: api/Citas/paciente/1
        [HttpGet("paciente/{idPaciente}")]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitasPorPaciente(int idPaciente)
        {
            if (idPaciente <= 0)
                return BadRequest("Id de paciente inválido.");

            var citas = await _context.Citas
                .Where(c => c.IdPaciente == idPaciente)
                .OrderBy(c => c.FechaInicio)
                .ToListAsync();

            if (citas == null || citas.Count == 0)
                return NotFound("El paciente no tiene citas registradas.");

            return Ok(citas);
        }

        // PUT: api/Citas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCita(int id, [FromBody] Cita citaUpdate)
        {
            if (id != citaUpdate.Id)
                return BadRequest("El ID de la cita no coincide.");

            var cita = await _context.Citas
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
                return NotFound("Cita no encontrada.");

            //  Campos que NO deberían poder modificarse directamente:
            // IdPaciente, IdPsicologo, ExpedienteId, IdDisponibilidad, FechaInicio, FechaFin

            // Solo actualizar campos permitidos
            cita.Estado = citaUpdate.Estado ?? cita.Estado;
            cita.Motivo = citaUpdate.Motivo ?? cita.Motivo;
            cita.FechaFin = citaUpdate.FechaFin;
            cita.FechaInicio = citaUpdate.FechaInicio;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitaExists(id))
                    return NotFound("La cita ya no existe.");
                else
                    throw;
            }

            return Ok(cita);
        }

        // POST: api/Citas
        [HttpPost]
        public async Task<ActionResult<Cita>> PostCita([FromBody] Cita cita)
        {
            if (cita == null)
                return BadRequest("Payload inválido.");

            if (cita.IdDisponibilidad <= 0)
                return BadRequest("IdDisponibilidad inválido.");

            if (cita.FechaInicio >= cita.FechaFin)
                return BadRequest("La hora de inicio debe ser menor que la hora de fin.");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Obtener disponibilidad con bloqueo para evitar conflictos
                    var disponibilidad = await _context.PsicologoDisponibilidad
                        .FromSqlRaw("SELECT * FROM PsicologoDisponibilidad WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", cita.IdDisponibilidad)
                        .AsTracking()
                        .FirstOrDefaultAsync();

                    if (disponibilidad == null)
                        return BadRequest("La disponibilidad indicada no existe.");

                    if (disponibilidad.IdPsicologo != cita.IdPsicologo)
                        return BadRequest("La disponibilidad no corresponde al psicólogo indicado.");

                    var diaObjetivo = disponibilidad.Fecha.Date;

                    // Validación paciente no tenga más de una cita ese día
                    bool pacienteTieneCita = await _context.Citas
                        .AnyAsync(c =>
                            c.IdPaciente == cita.IdPaciente &&
                            c.FechaInicio.Date == diaObjetivo
                        );

                    if (pacienteTieneCita)
                        return BadRequest("El paciente ya tiene una cita este día.");

                    // Validar traslapes con otras citas del psicólogo en ese día
                    bool traslape = await _context.Citas.AnyAsync(c =>
                        c.IdPsicologo == cita.IdPsicologo &&
                        c.FechaInicio.Date == diaObjetivo &&
                        (
                            // Inicio dentro de otra cita
                            (cita.FechaInicio >= c.FechaInicio && cita.FechaInicio < c.FechaFin) ||
                            // Fin dentro de otra cita
                            (cita.FechaFin > c.FechaInicio && cita.FechaFin <= c.FechaFin) ||
                            // Contiene completamente otra cita
                            (cita.FechaInicio <= c.FechaInicio && cita.FechaFin >= c.FechaFin)
                        )
                    );

                    if (traslape)
                        return BadRequest("Este horario ya esta ocupado.");

                    // Obtener/crear expediente
                    var expediente = await _context.Expediente
                        .FirstOrDefaultAsync(e => e.PacienteId == cita.IdPaciente);

                    if (expediente == null)
                    {
                        expediente = new Expediente { PacienteId = cita.IdPaciente };
                        _context.Expediente.Add(expediente);
                        await _context.SaveChangesAsync();
                    }

                    cita.ExpedienteId = expediente.Id;

                    // Guardar la cita
                    _context.Citas.Add(cita);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    await _hubContext.Clients.All.SendAsync("NewDate", cita);

                    return CreatedAtAction("GetCita", new { id = cita.Id }, cita);
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status409Conflict, "Conflicto de concurrencia, intenta de nuevo.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error interno al registrar cita.");
                }
            }
        }

        // DELETE: api/Citas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.Id == id);
        }
    }
}