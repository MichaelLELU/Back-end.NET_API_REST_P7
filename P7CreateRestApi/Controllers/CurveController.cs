using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Dto.CurvePoint;
using P7CreateRestApi.Dto.Common;
using P7CreateRestApi.Repositories.Interfaces;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,User")]
    public class CurveController : ControllerBase
    {
        private readonly ICurvePointRepository _curveRepository;
        private readonly ILogger<CurveController> _logger;

        public CurveController(
            ICurvePointRepository curveRepository,
            ILogger<CurveController> logger)
        {
            _curveRepository = curveRepository;
            _logger = logger;
        }

        // GET: api/Curve
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var curves = await _curveRepository.GetAllAsync();
            return Ok(curves);
        }

        // GET: api/Curve/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var curve = await _curveRepository.GetByIdAsync(id);
            if (curve is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Aucune courbe trouvée avec l'id {id}"
                });
            }

            return Ok(curve);
        }

        // POST: api/Curve
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCurvePointDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiErrorResponse.FromModelState(ModelState));
            }

            var curve = new CurvePoint
            {
                CurveId = dto.CurveId,
                Term = dto.Term,
                CurvePointValue = dto.CurvePointValue
            };

            await _curveRepository.AddAsync(curve);

            return CreatedAtAction(nameof(GetById), new { id = curve.Id }, curve);
        }

        // PUT: api/Curve/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCurvePointDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "L'identifiant ne correspond pas."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiErrorResponse.FromModelState(ModelState));
            }

            var curve = await _curveRepository.GetByIdAsync(id);
            if (curve is null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Aucune courbe trouvée avec l'id {id}"
                });
            }

            curve.CurveId = dto.CurveId;
            curve.Term = dto.Term;
            curve.CurvePointValue = dto.CurvePointValue;

            await _curveRepository.UpdateAsync(curve);

            return Ok(curve);
        }

        // DELETE: api/Curve/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _curveRepository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"Aucune courbe trouvée avec l'id {id}"
                });
            }

            return NoContent();
        }
    }
}
