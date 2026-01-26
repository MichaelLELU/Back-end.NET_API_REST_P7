using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Dto.Bid;
using P7CreateRestApi.Dto.Common;
using P7CreateRestApi.Repositories.Interfaces;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,User")]
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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bids = await _bidRepository.GetAllAsync();

            var result = bids.Select(b => new BidDto
            {
                Id = b.Id,
                Account = b.Account,
                BidType = b.BidType,
                BidQuantity = b.BidQuantity
            });

            return Ok(result);
        }

        // GET: api/Bid/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var bid = await _bidRepository.GetByIdAsync(id);
            if (bid is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Aucun Bid trouvé avec l'id {id}"
                });
            }

            var dto = new BidDto
            {
                Id = bid.Id,
                Account = bid.Account,
                BidType = bid.BidType,
                BidQuantity = bid.BidQuantity
            };

            return Ok(dto);
        }

        // POST: api/Bid
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBidDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiErrorResponse.FromModelState(ModelState));

            var bid = new Bid
            {
                Account = dto.Account,
                BidType = dto.BidType,
                BidQuantity = dto.BidQuantity
            };

            await _bidRepository.AddAsync(bid);

            return CreatedAtAction(
                nameof(GetById),
                new { id = bid.Id },
                new IdResponseDto { Id = bid.Id }
            );
        }

        // PUT: api/Bid/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBidDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "L'identifiant ne correspond pas."
                });
            }

            if (!ModelState.IsValid)
                return BadRequest(ApiErrorResponse.FromModelState(ModelState));

            var bid = await _bidRepository.GetByIdAsync(id);
            if (bid is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Aucun Bid trouvé avec l'id {id}"
                });
            }

            bid.Account = dto.Account;
            bid.BidType = dto.BidType;
            bid.BidQuantity = dto.BidQuantity;

            await _bidRepository.UpdateAsync(bid);

            var result = new BidDto
            {
                Id = bid.Id,
                Account = bid.Account,
                BidType = bid.BidType,
                BidQuantity = bid.BidQuantity
            };

            return Ok(result);
        }

        // DELETE: api/Bid/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _bidRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Aucun Bid trouvé avec l'id {id}"
                });
            }

            return NoContent();
        }
    }
}
