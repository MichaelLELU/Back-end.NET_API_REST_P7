using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class RatingRepository
    {
        private readonly LocalDbContext _context;

        public RatingRepository(LocalDbContext context)
        {
            _context = context;
        }

        // 🔹 Récupérer tous les Ratings
        public async Task<IEnumerable<Rating>> GetAllAsync()
        {
            return await _context.Ratings.ToListAsync();
        }

        // 🔹 Récupérer un Rating par ID
        public async Task<Rating> GetByIdAsync(int id)
        {
            return await _context.Ratings.FindAsync(id);
        }

        // 🔹 Ajouter un nouveau Rating
        public async Task<Rating> AddAsync(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return rating;
        }

        // 🔹 Mettre à jour un Rating existant
        public async Task<Rating> UpdateAsync(Rating rating)
        {
            _context.Entry(rating).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return rating;
        }

        // 🔹 Supprimer un Rating
        public async Task<bool> DeleteAsync(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
                return false;

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 Vérifier si un Rating existe
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Ratings.AnyAsync(r => r.Id == id);
        }
    }
}
