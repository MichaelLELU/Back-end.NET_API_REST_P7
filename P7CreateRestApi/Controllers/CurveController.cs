using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurveController : ControllerBase
    {
        private readonly LocalDbContext _context;

        public CurveController(LocalDbContext context)
        {
            _context = context;
        }

        // GET: api/Curve
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var curves = await _context.CurvePoints.ToListAsync();
            return Ok(curves);
        }

        // GET: api/Curve/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var curve = await _context.CurvePoints.FindAsync(id);
            if (curve == null)
                return NotFound(new { message = $"Aucune courbe trouvée avec l'id {id}" });

            return Ok(curve);
        }

        // POST: api/Curve
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CurvePoint curve)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (curve.Term == null || curve.CurvePointValue == null)
                return BadRequest(new { message = "Les champs Term et CurvePointValue sont requis." });

            _context.CurvePoints.Add(curve);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = curve.Id }, curve);
        }

        // PUT: api/Curve/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CurvePoint curve)
        {
            if (id != curve.Id)
                return BadRequest(new { message = "L'identifiant ne correspond pas." });

            var existingCurve = await _context.CurvePoints.FindAsync(id);
            if (existingCurve == null)
                return NotFound(new { message = $"Aucune courbe trouvée avec l'id {id}" });

            _context.Entry(existingCurve).CurrentValues.SetValues(curve);
            await _context.SaveChangesAsync();

            return Ok(existingCurve);
        }

        // DELETE: api/Curve/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var curve = await _context.CurvePoints.FindAsync(id);
            if (curve == null)
                return NotFound(new { message = $"Aucune courbe trouvée avec l'id {id}" });

            _context.CurvePoints.Remove(curve);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
