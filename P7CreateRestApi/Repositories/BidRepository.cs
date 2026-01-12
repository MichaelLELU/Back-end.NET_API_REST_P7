using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace P7CreateRestApi.Repositories
{
    public class BidRepository : IBidRepository
    {
        private readonly LocalDbContext _context;

        public BidRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bid>> GetAllAsync()
        {
            return await _context.Bids.AsNoTracking().ToListAsync();
        }

        public async Task<Bid?> GetByIdAsync(int id)
        {
            return await _context.Bids.AsNoTracking()
                                      .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Bid bid)
        {
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bid bid)
        {
            _context.Bids.Update(bid);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid is null)
                return false;

            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Bids.AnyAsync(b => b.Id == id);
        }
    }
}
