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
        public async Task<IActionResult> GetAll()
        {
            var trades = await _repository.GetAllAsync();
            return Ok(trades);
        }

        // GET: api/Trade/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trade = await _repository.GetByIdAsync(id);
            if (trade == null)
                return NotFound(new { message = $"Aucune transaction trouv�e avec l'id {id}" });

            return Ok(trade);
        }

        // POST: api/Trade
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Trade trade)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (trade.BuyQuantity == null && trade.SellQuantity == null)
                return BadRequest(new { message = "BuyQuantity ou SellQuantity doit �tre renseign�." });

            var createdTrade = await _repository.AddAsync(trade);
            return CreatedAtAction(nameof(GetById), new { id = createdTrade.TradeId }, createdTrade);
        }

        // PUT: api/Trade/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Trade trade)
        {
            if (id != trade.TradeId)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingTrade = await _repository.GetByIdAsync(id);
            if (existingTrade == null)
                return NotFound(new { message = $"Aucune transaction trouv�e avec l'id {id}" });

            await _repository.UpdateAsync(trade);
            return Ok(trade);
        }

        // DELETE: api/Trade/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Aucune transaction trouv�e avec l'id {id}" });

            return NoContent();
        }
    }
}
