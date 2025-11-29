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

        // POST: api/Citas
        [HttpPost]
        public async Task<ActionResult<Cita>> PostCita([FromBody] Cita cita)
        {
            if (cita == null)
                return BadRequest("Payload inválido.");

            if (cita.IdDisponibilidad <= 0)
                return BadRequest("IdDisponibilidad inválido.");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var disponibilidad = await _context.PsicologoDisponibilidad
                        .FromSqlRaw("SELECT * FROM PsicologoDisponibilidad WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", cita.IdDisponibilidad)
                        .AsTracking()
                        .FirstOrDefaultAsync();

                    if (disponibilidad == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("La disponibilidad indicada no existe.");
                    }

                    if (disponibilidad.IdPsicologo != cita.IdPsicologo)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("La disponibilidad no corresponde al psicólogo indicado.");
                    }

                    var diaObjetivo = disponibilidad.Fecha.Date;

                    // REGLA: Un paciente solo puede tener 1 cita ese día
                    bool pacienteTieneCita = await _context.Citas
                        .AnyAsync(c => c.IdPaciente == cita.IdPaciente && c.FechaInicio.Date == diaObjetivo);

                    if (pacienteTieneCita)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("El paciente ya tiene una cita este día.");
                    }

                    // BLOQUES DE HORARIO AUTOMÁTICOS
                    int citasPrevias = disponibilidad.CitasAgendadas;

                    if (citasPrevias >= 4)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest("No hay cupos disponibles para este día.");
                    }

                    DateTime fechaBase = disponibilidad.Fecha.Date;
                    TimeSpan horaInicioBase = new TimeSpan(7, 0, 0); // 7am
                    TimeSpan duracion = new TimeSpan(2, 0, 0);      // 2h por cita

                    var fechaInicio = fechaBase + horaInicioBase + TimeSpan.FromTicks(duracion.Ticks * citasPrevias);
                    var fechaFin = fechaInicio + duracion;

                    cita.FechaInicio = fechaInicio;
                    cita.FechaFin = fechaFin;

                    var expediente = await _context.Expediente
                        .FirstOrDefaultAsync(e => e.PacienteId == cita.IdPaciente);

                    if (expediente == null)
                    {
                        expediente = new Expediente { PacienteId = cita.IdPaciente };
                        _context.Expediente.Add(expediente);
                        await _context.SaveChangesAsync();
                    }

                    cita.ExpedienteId = expediente.Id;

                    _context.Citas.Add(cita);

                    disponibilidad.CitasAgendadas += 1;
                    _context.PsicologoDisponibilidad.Update(disponibilidad);

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
                catch (Exception)
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