using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class BidRepository
    {
        private readonly LocalDbContext _context;

        public BidRepository(LocalDbContext context)
        {
            _context = context;
        }

        // 🔹 Récupérer tous les Bids
        public async Task<IEnumerable<Bid>> GetAllAsync()
        {
            return await _context.Bids.ToListAsync();
        }

        // 🔹 Récupérer un Bid par ID
        public async Task<Bid> GetByIdAsync(int id)
        {
            return await _context.Bids.FindAsync(id);
        }

        // 🔹 Créer un nouveau Bid
        public async Task<Bid> AddAsync(Bid bid)
        {
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
            return bid;
        }

        // 🔹 Mettre à jour un Bid existant
        public async Task<Bid> UpdateAsync(Bid bid)
        {
            _context.Entry(bid).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return bid;
        }

        // 🔹 Supprimer un Bid
        public async Task<bool> DeleteAsync(int id)
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null)
                return false;

            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 Vérifier l’existence d’un Bid
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Bids.AnyAsync(b => b.Id == id);
        }
    }
}
