using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurveController : ControllerBase
    {
        private readonly ICurvePointRepository _curveRepository;
        private readonly ILogger<CurveController> _logger;

        public CurveController(
            ICurvePointRepository curveRepository,
            ILogger<CurveController> logger)
        {
            _curveRepository = curveRepository;
            _logger = logger;
        }

        // GET: api/Curve
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var curves = await _curveRepository.GetAllAsync();
            _logger.LogInformation("Liste des courbes récupérée");
            return Ok(curves);
        }

        // GET: api/Curve/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var curve = await _curveRepository.GetByIdAsync(id);
            if (curve is null)
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

            if (curve.Term is null || curve.CurvePointValue is null)
            {
                _logger.LogWarning("Échec de création : champs manquants pour CurvePoint");
                return BadRequest(new
                {
                    message = "Les champs Term et CurvePointValue sont requis."
                });
            }

            await _curveRepository.AddAsync(curve);

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

            var existingCurve = await _curveRepository.GetByIdAsync(id);
            if (existingCurve is null)
            {
                _logger.LogWarning("Échec de mise à jour : courbe {Id} introuvable", id);
                return NotFound(new { message = $"Aucune courbe trouvée avec l'id {id}" });
            }

            await _curveRepository.UpdateAsync(curve);

            _logger.LogInformation("Courbe {Id} mise à jour par {User}", id, User.Identity?.Name);
            return Ok(curve);
        }

        // DELETE: api/Curve/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _curveRepository.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning(
                    "Tentative de suppression d'une courbe inexistante (id {Id})",
                    id);
                return NotFound(new { message = $"Aucune courbe trouvée avec l'id {id}" });
            }

            _logger.LogWarning("Courbe {Id} supprimée par {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
