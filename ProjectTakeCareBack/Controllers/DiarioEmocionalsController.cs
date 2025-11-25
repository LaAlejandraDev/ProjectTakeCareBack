using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTakeCareBack.Data;
using ProjectTakeCareBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace ProjectTakeCareBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiarioEmocionalsController : ControllerBase
    {
        private readonly TakeCareContext _context;
        private readonly HttpClient _httpClient;

        public DiarioEmocionalsController(TakeCareContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: api/DiarioEmocionals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiarioEmocional>>> GetDiarioEmocional()
        {
            return await _context.DiarioEmocional.ToListAsync();
        }

        // GET: api/DiarioEmocionals/paciente/10
        [HttpGet("paciente/{idPaciente}")]
        public async Task<ActionResult<IEnumerable<DiarioEmocional>>> GetDiariosByPaciente(int idPaciente)
        {
            try
            {
                var diarios = await _context.DiarioEmocional
                    .Where(d => d.IdPaciente == idPaciente)
                    .OrderByDescending(d => d.Fecha)
                    .ToListAsync();

                if (diarios == null || diarios.Count == 0)
                {
                    return NotFound(new { message = "El paciente no tiene diarios registrados." });
                }

                return Ok(diarios);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetDiariosByPaciente:", ex);
                return StatusCode(500, "Error al obtener los diarios del paciente.");
            }
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
            try
            {
                // 1. Preparar cliente HTTP
                using var http = new HttpClient();
                http.BaseAddress = new Uri("http://localhost:8001"); // URL de tu API Node.js

                // 2. Crear body para enviar al endpoint /interpretar
                var requestBody = new
                {
                    texto = diarioEmocional.Comentario ?? ""
                };

                // 3. Llamar a la API Node.js
                var response = await http.PostAsJsonAsync("/interpretar", requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error interpretando emoción desde Node.js");
                    return StatusCode(500, "Error interpretando estado emocional");
                }

                // 4. Recibir el texto interpretado
                var data = await response.Content.ReadFromJsonAsync<InterpretacionResponse>();

                // 5. Asignar la interpretación a tu modelo
                diarioEmocional.EstadoEmocional = data!.resultado;

                // 6. Guardar en base de datos
                _context.DiarioEmocional.Add(diarioEmocional);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetDiarioEmocional", new { id = diarioEmocional.Id }, diarioEmocional);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error general:", ex);
                return StatusCode(500, "Error en el servicio");
            }
        }

        // Modelo auxiliar para recibir JSON
        public class InterpretacionResponse
        {
            public string resultado { get; set; } = "";
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
