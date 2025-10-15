using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using System.Threading.Tasks;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RuleNameController : ControllerBase
    {
        private readonly RuleNameRepository _repository;

        public RuleNameController(RuleNameRepository repository)
        {
            _repository = repository;
        }

        // GET: api/RuleName
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _repository.GetAllAsync();
            return Ok(rules);
        }

        // GET: api/RuleName/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rule = await _repository.GetByIdAsync(id);
            if (rule == null)
                return NotFound(new { message = $"Aucune règle trouvée avec l'id {id}" });

            return Ok(rule);
        }

        // POST: api/RuleName
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RuleName rule)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(rule.Name))
                return BadRequest(new { message = "Le champ Name est requis." });

            var createdRule = await _repository.AddAsync(rule);
            return CreatedAtAction(nameof(GetById), new { id = createdRule.Id }, createdRule);
        }

        // PUT: api/RuleName/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RuleName rule)
        {
            if (id != rule.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingRule = await _repository.GetByIdAsync(id);
            if (existingRule == null)
                return NotFound(new { message = $"Aucune règle trouvée avec l'id {id}" });

            await _repository.UpdateAsync(rule);
            return Ok(rule);
        }

        // DELETE: api/RuleName/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Aucune règle trouvée avec l'id {id}" });

            return NoContent();
        }
    }
}
