using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class TradeRepository
    {
        private readonly LocalDbContext _context;

        public TradeRepository(LocalDbContext context)
        {
            _context = context;
        }

        // 🔹 Récupérer tous les Trades
        public async Task<IEnumerable<Trade>> GetAllAsync()
        {
            return await _context.Trades.ToListAsync();
        }

        // 🔹 Récupérer un Trade par ID
        public async Task<Trade> GetByIdAsync(int id)
        {
            return await _context.Trades.FindAsync(id);
        }

        // 🔹 Ajouter un nouveau Trade
        public async Task<Trade> AddAsync(Trade trade)
        {
            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();
            return trade;
        }

        // 🔹 Mettre à jour un Trade existant
        public async Task<Trade> UpdateAsync(Trade trade)
        {
            _context.Entry(trade).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return trade;
        }

        // 🔹 Supprimer un Trade
        public async Task<bool> DeleteAsync(int id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade == null)
                return false;

            _context.Trades.Remove(trade);
            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 Vérifier si un Trade existe
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Trades.AnyAsync(t => t.TradeId == id);
        }
    }
}
