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

        public RatingController(LocalDbContext context)
        {
            _context = context;
        }

        // GET: api/Rating
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ratings = await _context.Ratings.ToListAsync();
            return Ok(ratings);
        }

        // GET: api/Rating/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
                return NotFound(new { message = $"Aucun Rating trouvé avec l'id {id}" });

            return Ok(rating);
        }

        // POST: api/Rating
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Rating rating)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(rating.MoodysRating) &&
                string.IsNullOrWhiteSpace(rating.SandPRating) &&
                string.IsNullOrWhiteSpace(rating.FitchRating))
            {
                return BadRequest(new { message = "Au moins une notation (Moody's, S&P ou Fitch) est requise." });
            }

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
        }

        // PUT: api/Rating/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Rating rating)
        {
            if (id != rating.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingRating = await _context.Ratings.FindAsync(id);
            if (existingRating == null)
                return NotFound(new { message = $"Aucun Rating trouvé avec l'id {id}" });

            _context.Entry(existingRating).CurrentValues.SetValues(rating);
            await _context.SaveChangesAsync();

            return Ok(existingRating);
        }

        // DELETE: api/Rating/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
                return NotFound(new { message = $"Aucun Rating trouvé avec l'id {id}" });

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
