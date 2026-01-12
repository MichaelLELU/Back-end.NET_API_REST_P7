using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace P7CreateRestApi.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly LocalDbContext _context;

        public TradeRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trade>> GetAllAsync()
        {
            return await _context.Trades
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Trade?> GetByIdAsync(int id)
        {
            return await _context.Trades
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(Trade trade)
        {
            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Trade trade)
        {
            _context.Trades.Update(trade);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade is null)
                return false;

            _context.Trades.Remove(trade);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Trades.AnyAsync(t => t.Id == id);
        }
    }
}
