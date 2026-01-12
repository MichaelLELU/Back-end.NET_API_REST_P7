using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RuleNameController : ControllerBase
    {
        private readonly IRuleNameRepository _ruleNameRepository;
        private readonly ILogger<RuleNameController> _logger;

        public RuleNameController(
            IRuleNameRepository ruleNameRepository,
            ILogger<RuleNameController> logger)
        {
            _ruleNameRepository = ruleNameRepository;
            _logger = logger;
        }

        // GET: api/RuleName
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _ruleNameRepository.GetAllAsync();
            _logger.LogInformation("Liste des règles récupérée");
            return Ok(rules);
        }

        // GET: api/RuleName/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id)
        {
            var rule = await _ruleNameRepository.GetByIdAsync(id);
            if (rule is null)
            {
                _logger.LogWarning("Aucune règle trouvée avec l'id {Id}", id);
                return NotFound(new { message = $"Aucune règle trouvée avec l'id {id}" });
            }

            _logger.LogInformation("Règle {Id} consultée", id);
            return Ok(rule);
        }

        // POST: api/RuleName
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Create([FromBody] RuleName rule)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(rule.Name))
            {
                _logger.LogWarning("Échec de création : champ Name manquant");
                return BadRequest(new { message = "Le champ Name est requis." });
            }

            await _ruleNameRepository.AddAsync(rule);

            _logger.LogInformation(
                "Règle {Id} créée par {User}",
                rule.Id,
                User.Identity?.Name);

            return CreatedAtAction(nameof(GetById), new { id = rule.Id }, rule);
        }

        // PUT: api/RuleName/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(int id, [FromBody] RuleName rule)
        {
            if (id != rule.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            if (!await _ruleNameRepository.ExistsAsync(id))
            {
                _logger.LogWarning("Échec de mise à jour : règle {Id} introuvable", id);
                return NotFound(new { message = $"Aucune règle trouvée avec l'id {id}" });
            }

            await _ruleNameRepository.UpdateAsync(rule);

            _logger.LogInformation("Règle {Id} mise à jour par {User}", id, User.Identity?.Name);
            return Ok(rule);
        }

        // DELETE: api/RuleName/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _ruleNameRepository.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Tentative de suppression d’une règle inexistante ({Id})", id);
                return NotFound(new { message = $"Aucune règle trouvée avec l'id {id}" });
            }

            _logger.LogWarning("Règle {Id} supprimée par {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
