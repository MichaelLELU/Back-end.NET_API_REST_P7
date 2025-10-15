using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public class RuleNameRepository
    {
        private readonly LocalDbContext _context;

        public RuleNameRepository(LocalDbContext context)
        {
            _context = context;
        }

        // 🔹 Récupérer tous les RuleNames
        public async Task<IEnumerable<RuleName>> GetAllAsync()
        {
            return await _context.RuleNames.ToListAsync();
        }

        // 🔹 Récupérer un RuleName par ID
        public async Task<RuleName> GetByIdAsync(int id)
        {
            return await _context.RuleNames.FindAsync(id);
        }

        // 🔹 Ajouter un nouveau RuleName
        public async Task<RuleName> AddAsync(RuleName rule)
        {
            _context.RuleNames.Add(rule);
            await _context.SaveChangesAsync();
            return rule;
        }

        // 🔹 Mettre à jour un RuleName existant
        public async Task<RuleName> UpdateAsync(RuleName rule)
        {
            _context.Entry(rule).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return rule;
        }

        // 🔹 Supprimer un RuleName
        public async Task<bool> DeleteAsync(int id)
        {
            var rule = await _context.RuleNames.FindAsync(id);
            if (rule == null)
                return false;

            _context.RuleNames.Remove(rule);
            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 Vérifier l’existence d’un RuleName
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.RuleNames.AnyAsync(r => r.Id == id);
        }
    }
}
