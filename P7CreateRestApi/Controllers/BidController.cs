using Dot.Net.WebApi.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BidController : ControllerBase
    {
        [HttpGet]
        [Route("validate")]
        public IActionResult Validate([FromBody] Bid bidList)
        {
            // TODO: check data valid and save to db, after saving return bid list
            return Ok();
        }

        [HttpGet]
        [Route("update/{id}")]
        public IActionResult ShowUpdateForm(int id)
        {
            return Ok();
        }

        [HttpPost]
        [Route("update/{id}")]
        public IActionResult UpdateBid(int id, [FromBody] Bid bidList)
        {
            // TODO: check required fields, if valid call service to update Bid and return list Bid
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteBid(int id)
        {
            return Ok();
        }
    }
}