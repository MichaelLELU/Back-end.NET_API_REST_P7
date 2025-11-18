using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using System.Threading.Tasks;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly TradeRepository _repository;
        private readonly ILogger<TradeController> _logger;

        public TradeController(TradeRepository repository, ILogger<TradeController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // --- LECTURE DE TOUTES LES TRANSACTIONS ---
        // GET: api/Trade
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll()
        {
            var trades = await _repository.GetAllAsync();
            _logger.LogInformation("Liste des transactions récupérée ({Count} entrées)", trades.Count());
            return Ok(trades);
        }

        // --- LECTURE PAR ID ---
        // GET: api/Trade/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id)
        {
            var trade = await _repository.GetByIdAsync(id);
            if (trade == null)
            {
                _logger.LogWarning("Aucune transaction trouvée avec l'id {Id}", id);
                return NotFound(new { message = $"Aucune transaction trouvée avec l'id {id}" });
            }

            _logger.LogInformation("Transaction {Id} consultée", id);
            return Ok(trade);
        }

        // --- CRÉATION ---
        // POST: api/Trade
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([FromBody] Trade trade)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (trade.BuyQuantity == null && trade.SellQuantity == null)
            {
                _logger.LogWarning("Échec de création : quantités manquantes pour la transaction");
                return BadRequest(new { message = "BuyQuantity ou SellQuantity doit être renseigné." });
            }

            var createdTrade = await _repository.AddAsync(trade);

            _logger.LogInformation("Transaction {Id} créée par {User}", createdTrade.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = createdTrade.Id }, createdTrade);
        }

        // --- MISE À JOUR ---
        // PUT: api/Trade/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(int id, [FromBody] Trade trade)
        {
            if (id != trade.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingTrade = await _repository.GetByIdAsync(id);
            if (existingTrade == null)
            {
                _logger.LogWarning("Échec de mise à jour : transaction {Id} introuvable", id);
                return NotFound(new { message = $"Aucune transaction trouvée avec l'id {id}" });
            }

            await _repository.UpdateAsync(trade);

            _logger.LogInformation("Transaction {Id} mise à jour par {User}", id, User.Identity?.Name);
            return Ok(trade);
        }

        // --- SUPPRESSION ---
        // DELETE: api/Trade/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Tentative de suppression d’une transaction inexistante ({Id})", id);
                return NotFound(new { message = $"Aucune transaction trouvée avec l'id {id}" });
            }

            _logger.LogWarning("Transaction {Id} supprimée par {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
