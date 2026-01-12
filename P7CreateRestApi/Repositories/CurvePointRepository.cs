using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace P7CreateRestApi.Repositories
{
    public class CurvePointRepository : ICurvePointRepository
    {
        private readonly LocalDbContext _context;

        public CurvePointRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CurvePoint>> GetAllAsync()
        {
            return await _context.CurvePoints
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<CurvePoint?> GetByIdAsync(int id)
        {
            return await _context.CurvePoints
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(CurvePoint curvePoint)
        {
            _context.CurvePoints.Add(curvePoint);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CurvePoint curvePoint)
        {
            _context.CurvePoints.Update(curvePoint);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var curvePoint = await _context.CurvePoints.FindAsync(id);
            if (curvePoint is null)
                return false;

            _context.CurvePoints.Remove(curvePoint);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
