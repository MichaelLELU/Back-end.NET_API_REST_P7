using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class CurvePointRepository
    {
        private readonly LocalDbContext _context;

        public CurvePointRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CurvePoint>> GetAllAsync()
        {
            return await _context.CurvePoints.ToListAsync();
        }

        public async Task<CurvePoint> GetByIdAsync(int id)
        {
            return await _context.CurvePoints.FindAsync(id);
        }

        public async Task<CurvePoint> AddAsync(CurvePoint curve)
        {
            _context.CurvePoints.Add(curve);
            await _context.SaveChangesAsync();
            return curve;
        }

        public async Task<CurvePoint> UpdateAsync(CurvePoint curve)
        {
            _context.Entry(curve).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return curve;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var curve = await _context.CurvePoints.FindAsync(id);
            if (curve == null)
                return false;

            _context.CurvePoints.Remove(curve);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
