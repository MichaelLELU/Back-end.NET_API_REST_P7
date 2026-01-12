using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidController : ControllerBase
    {
        private readonly IBidRepository _bidRepository;
        private readonly ILogger<BidController> _logger;

        public BidController(
            IBidRepository bidRepository,
            ILogger<BidController> logger)
        {
            _bidRepository = bidRepository;
            _logger = logger;
        }

        // GET: api/Bid
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bids = await _bidRepository.GetAllAsync();
            _logger.LogInformation("Récupération de tous les Bids");
            return Ok(bids);
        }

        // GET: api/Bid/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bid = await _bidRepository.GetByIdAsync(id);
            if (bid is null)
            {
                _logger.LogWarning("Aucun Bid trouvé avec l'id {Id}", id);
                return NotFound(new { message = $"Aucun Bid avec l'id {id}" });
            }

            _logger.LogInformation("Lecture du Bid {Id}", id);
            return Ok(bid);
        }

        // POST: api/Bid
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Bid bid)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(bid.Account))
                return BadRequest(new { message = "Le champ Account est requis." });

            await _bidRepository.AddAsync(bid);

            _logger.LogInformation("Bid {Id} créé par {User}", bid.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = bid.Id }, bid);
        }

        // PUT: api/Bid/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Bid bid)
        {
            if (id != bid.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            if (!await _bidRepository.ExistsAsync(id))
                return NotFound(new { message = $"Aucun Bid trouvé avec l'id {id}" });

            await _bidRepository.UpdateAsync(bid);

            _logger.LogInformation("Bid {Id} modifié par {User}", id, User.Identity?.Name);
            return Ok(bid);
        }

        // DELETE: api/Bid/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bidRepository.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Tentative de suppression d’un Bid inexistant ({Id})", id);
                return NotFound(new { message = $"Aucun Bid trouvé avec l'id {id}" });
            }

            _logger.LogWarning("Bid {Id} supprimé par {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
