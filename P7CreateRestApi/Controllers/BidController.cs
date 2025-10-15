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

        public BidController(LocalDbContext context)
        {
            _context = context;
        }

        // GET: api/Bid
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bids = await _context.Bids.ToListAsync();
            return Ok(bids);
        }

        // GET: api/Bid/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null)
                return NotFound(new { message = $"Aucun Bid avec l'id {id}" });
            return Ok(bid);
        }

        // POST: api/Bid
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Bid bid)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validation basique (exemple)
            if (string.IsNullOrWhiteSpace(bid.Account))
                return BadRequest(new { message = "Le champ Account est requis." });

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = bid.BidListId }, bid);
        }

        // PUT: api/Bid/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Bid bid)
        {
            if (id != bid.BidListId)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingBid = await _context.Bids.FindAsync(id);
            if (existingBid == null)
                return NotFound(new { message = $"Aucun Bid trouvé avec l'id {id}" });

            // Mise à jour sécurisée des champs
            _context.Entry(existingBid).CurrentValues.SetValues(bid);
            await _context.SaveChangesAsync();

            return Ok(existingBid);
        }

        // DELETE: api/Bid/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null)
                return NotFound(new { message = $"Aucun Bid trouvé avec l'id {id}" });

            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
