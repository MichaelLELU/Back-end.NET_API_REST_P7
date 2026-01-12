using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly ILogger<TradeController> _logger;

        public TradeController(
            ITradeRepository tradeRepository,
            ILogger<TradeController> logger)
        {
            _tradeRepository = tradeRepository;
            _logger = logger;
        }

        // GET: api/Trade
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll()
        {
            var trades = await _tradeRepository.GetAllAsync();
            _logger.LogInformation("Liste des transactions récupérée");
            return Ok(trades);
        }

        // GET: api/Trade/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id)
        {
            var trade = await _tradeRepository.GetByIdAsync(id);
            if (trade is null)
            {
                _logger.LogWarning("Aucune transaction trouvée avec l'id {Id}", id);
                return NotFound(new { message = $"Aucune transaction trouvée avec l'id {id}" });
            }

            _logger.LogInformation("Transaction {Id} consultée", id);
            return Ok(trade);
        }

        // POST: api/Trade
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([FromBody] Trade trade)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (trade.BuyQuantity is null && trade.SellQuantity is null)
            {
                _logger.LogWarning("Échec de création : quantités manquantes pour la transaction");
                return BadRequest(new
                {
                    message = "BuyQuantity ou SellQuantity doit être renseigné."
                });
            }

            await _tradeRepository.AddAsync(trade);

            _logger.LogInformation(
                "Transaction {Id} créée par {User}",
                trade.Id,
                User.Identity?.Name);

            return CreatedAtAction(nameof(GetById), new { id = trade.Id }, trade);
        }

        // PUT: api/Trade/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(int id, [FromBody] Trade trade)
        {
            if (id != trade.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            if (!await _tradeRepository.ExistsAsync(id))
            {
                _logger.LogWarning("Échec de mise à jour : transaction {Id} introuvable", id);
                return NotFound(new { message = $"Aucune transaction trouvée avec l'id {id}" });
            }

            await _tradeRepository.UpdateAsync(trade);

            _logger.LogInformation("Transaction {Id} mise à jour par {User}", id, User.Identity?.Name);
            return Ok(trade);
        }

        // DELETE: api/Trade/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _tradeRepository.DeleteAsync(id);
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
