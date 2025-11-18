using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidController : ControllerBase
    {
        private readonly LocalDbContext _context;
        private readonly ILogger<BidController> _logger;

        public BidController(LocalDbContext context, ILogger<BidController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // --- LECTURE PUBLIQUE ---
        // GET: api/Bid
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bids = await _context.Bids.ToListAsync();
            _logger.LogInformation("Récupération de tous les Bids ({Count})", bids.Count);
            return Ok(bids);
        }

        // --- LECTURE PAR ID ---
        // GET: api/Bid/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null)
            {
                _logger.LogWarning("Aucun Bid trouvé avec l'id {Id}", id);
                return NotFound(new { message = $"Aucun Bid avec l'id {id}" });
            }

            _logger.LogInformation("Lecture du Bid {Id}", id);
            return Ok(bid);
        }

        // --- CRÉATION ---
        // POST: api/Bid
        [Authorize(Roles = "Admin,User")] 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Bid bid)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(bid.Account))
                return BadRequest(new { message = "Le champ Account est requis." });

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Bid {Id} créé par {User}", bid.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = bid.Id }, bid);
        }

        // --- MISE À JOUR ---
        // PUT: api/Bid/{id}
        [Authorize(Roles = "Admin,User")] 
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Bid bid)
        {
            if (id != bid.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingBid = await _context.Bids.FindAsync(id);
            if (existingBid == null)
                return NotFound(new { message = $"Aucun Bid trouvé avec l'id {id}" });

            _context.Entry(existingBid).CurrentValues.SetValues(bid);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Bid {Id} modifié par {User}", id, User.Identity?.Name);
            return Ok(existingBid);
        }

        // --- SUPPRESSION ---
        // DELETE: api/Bid/{id}
        [Authorize(Roles = "Admin")] 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null)
            {
                _logger.LogWarning("Tentative de suppression d’un Bid inexistant ({Id})", id);
                return NotFound(new { message = $"Aucun Bid trouvé avec l'id {id}" });
            }

            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Bid {Id} supprimé par {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
