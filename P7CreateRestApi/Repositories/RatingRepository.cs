using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace P7CreateRestApi.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly LocalDbContext _context;

        public RatingRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rating>> GetAllAsync()
        {
            return await _context.Ratings
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Rating?> GetByIdAsync(int id)
        {
            return await _context.Ratings
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Rating rating)
        {
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating is null)
                return false;

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Ratings.AnyAsync(r => r.Id == id);
        }
    }
}
