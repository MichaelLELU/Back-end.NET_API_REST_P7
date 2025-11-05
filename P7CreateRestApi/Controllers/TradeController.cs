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

        public TradeController(TradeRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Trade
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll()
        {
            var trades = await _repository.GetAllAsync();
            return Ok(trades);
        }

        // GET: api/Trade/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id)
        {
            var trade = await _repository.GetByIdAsync(id);
            if (trade == null)
                return NotFound(new { message = $"Aucune transaction trouvée avec l'id {id}" });

            return Ok(trade);
        }

        // POST: api/Trade
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([FromBody] Trade trade)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (trade.BuyQuantity == null && trade.SellQuantity == null)
                return BadRequest(new { message = "BuyQuantity ou SellQuantity doit être renseigné." });

            var createdTrade = await _repository.AddAsync(trade);
            return CreatedAtAction(nameof(GetById), new { id = createdTrade.Id }, createdTrade);
        }

        // PUT: api/Trade/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(int id, [FromBody] Trade trade)
        {
            if (id != trade.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingTrade = await _repository.GetByIdAsync(id);
            if (existingTrade == null)
                return NotFound(new { message = $"Aucune transaction trouvée avec l'id {id}" });

            await _repository.UpdateAsync(trade);
            return Ok(trade);
        }

        // DELETE: api/Trade/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Aucune transaction trouvée avec l'id {id}" });

            return NoContent();
        }
    }
}
