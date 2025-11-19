using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurveController : ControllerBase
    {
        private readonly LocalDbContext _context;
        private readonly ILogger<CurveController> _logger;

        public CurveController(LocalDbContext context, ILogger<CurveController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Curve
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var curves = await _context.CurvePoints.ToListAsync();
            _logger.LogInformation("Liste des courbes récupérée ({Count} entrées)", curves.Count);
            return Ok(curves);
        }

        // GET: api/Curve/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var curve = await _context.CurvePoints.FindAsync(id);
            if (curve == null)
            {
                _logger.LogWarning("Aucune courbe trouvée avec l'id {Id}", id);
                return NotFound(new { message = $"Aucune courbe trouvée avec l'id {id}" });
            }

            _logger.LogInformation("Courbe {Id} consultée", id);
            return Ok(curve);
        }

        // POST: api/Curve
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CurvePoint curve)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (curve.Term == null || curve.CurvePointValue == null)
            {
                _logger.LogWarning("Échec de création : champs manquants pour CurvePoint");
                return BadRequest(new { message = "Les champs Term et CurvePointValue sont requis." });
            }

            _context.CurvePoints.Add(curve);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Courbe {Id} créée par {User}", curve.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = curve.Id }, curve);
        }

        // PUT: api/Curve/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CurvePoint curve)
        {
            if (id != curve.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingCurve = await _context.CurvePoints.FindAsync(id);
            if (existingCurve == null)
            {
                _logger.LogWarning("Échec de mise à jour : courbe {Id} introuvable", id);
                return NotFound(new { message = $"Aucune courbe trouvée avec l'id {id}" });
            }

            _context.Entry(existingCurve).CurrentValues.SetValues(curve);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Courbe {Id} mise à jour par {User}", id, User.Identity?.Name);
            return Ok(existingCurve);
        }

        // DELETE: api/Curve/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var curve = await _context.CurvePoints.FindAsync(id);
            if (curve == null)
            {
                _logger.LogWarning("Tentative de suppression d'une courbe inexistante (id {Id})", id);
                return NotFound(new { message = $"Aucune courbe trouvée avec l'id {id}" });
            }

            _context.CurvePoints.Remove(curve);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Courbe {Id} supprimée par {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
