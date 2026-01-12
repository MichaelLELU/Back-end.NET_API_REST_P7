using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly ILogger<RatingController> _logger;

        public RatingController(
            IRatingRepository ratingRepository,
            ILogger<RatingController> logger)
        {
            _ratingRepository = ratingRepository;
            _logger = logger;
        }

        // GET: api/Rating
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ratings = await _ratingRepository.GetAllAsync();
            _logger.LogInformation("Liste des ratings récupérée");
            return Ok(ratings);
        }

        // GET: api/Rating/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rating = await _ratingRepository.GetByIdAsync(id);
            if (rating is null)
            {
                _logger.LogWarning("Aucun rating trouvé avec l'id {Id}", id);
                return NotFound(new { message = $"Aucun Rating trouvé avec l'id {id}" });
            }

            _logger.LogInformation("Rating {Id} consulté", id);
            return Ok(rating);
        }

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
                return BadRequest(new
                {
                    message = "Au moins une notation (Moody's, S&P ou Fitch) est requise."
                });
            }

            await _ratingRepository.AddAsync(rating);

            _logger.LogInformation("Rating {Id} créé par {User}", rating.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
        }

        // PUT: api/Rating/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Rating rating)
        {
            if (id != rating.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            if (!await _ratingRepository.ExistsAsync(id))
            {
                _logger.LogWarning("Échec de mise à jour : rating {Id} introuvable", id);
                return NotFound(new { message = $"Aucun Rating trouvé avec l'id {id}" });
            }

            await _ratingRepository.UpdateAsync(rating);

            _logger.LogInformation("Rating {Id} mis à jour par {User}", id, User.Identity?.Name);
            return Ok(rating);
        }

        // DELETE: api/Rating/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _ratingRepository.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Tentative de suppression d’un rating inexistant ({Id})", id);
                return NotFound(new { message = $"Aucun Rating trouvé avec l'id {id}" });
            }

            _logger.LogWarning("Rating {Id} supprimé par {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
