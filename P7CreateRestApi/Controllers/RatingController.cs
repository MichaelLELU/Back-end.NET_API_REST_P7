using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly LocalDbContext _context;
        private readonly ILogger<RatingController> _logger;

        public RatingController(LocalDbContext context, ILogger<RatingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // --- LECTURE DE TOUS LES RATINGS ---
        // GET: api/Rating
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ratings = await _context.Ratings.ToListAsync();
            _logger.LogInformation("Liste des ratings récupérée ({Count} entrées)", ratings.Count);
            return Ok(ratings);
        }

        // --- LECTURE PAR ID ---
        // GET: api/Rating/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                _logger.LogWarning("Aucun rating trouvé avec l'id {Id}", id);
                return NotFound(new { message = $"Aucun Rating trouvé avec l'id {id}" });
            }

            _logger.LogInformation("Rating {Id} consulté", id);
            return Ok(rating);
        }

        // --- CRÉATION ---
        // POST: api/Rating
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Rating rating)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(rating.MoodysRating) &&
                string.IsNullOrWhiteSpace(rating.SandPRating) &&
                string.IsNullOrWhiteSpace(rating.FitchRating))
            {
                _logger.LogWarning("Échec de création : aucune notation fournie pour le rating");
                return BadRequest(new { message = "Au moins une notation (Moody's, S&P ou Fitch) est requise." });
            }

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rating {Id} créé par {User}", rating.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
        }

        // --- MISE À JOUR ---
        // PUT: api/Rating/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Rating rating)
        {
            if (id != rating.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingRating = await _context.Ratings.FindAsync(id);
            if (existingRating == null)
            {
                _logger.LogWarning("Échec de mise à jour : rating {Id} introuvable", id);
                return NotFound(new { message = $"Aucun Rating trouvé avec l'id {id}" });
            }

            _context.Entry(existingRating).CurrentValues.SetValues(rating);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rating {Id} mis à jour par {User}", id, User.Identity?.Name);
            return Ok(existingRating);
        }

        // --- SUPPRESSION ---
        // DELETE: api/Rating/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                _logger.LogWarning("Tentative de suppression d’un rating inexistant ({Id})", id);
                return NotFound(new { message = $"Aucun Rating trouvé avec l'id {id}" });
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Rating {Id} supprimé par {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
