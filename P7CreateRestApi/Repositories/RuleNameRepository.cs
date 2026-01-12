using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories.Interfaces;

namespace P7CreateRestApi.Repositories
{
    public class RuleNameRepository : IRuleNameRepository
    {
        private readonly LocalDbContext _context;

        public RuleNameRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RuleName>> GetAllAsync()
        {
            return await _context.RuleNames
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<RuleName?> GetByIdAsync(int id)
        {
            return await _context.RuleNames
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(RuleName ruleName)
        {
            _context.RuleNames.Add(ruleName);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RuleName ruleName)
        {
            _context.RuleNames.Update(ruleName);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ruleName = await _context.RuleNames.FindAsync(id);
            if (ruleName is null)
                return false;

            _context.RuleNames.Remove(ruleName);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.RuleNames.AnyAsync(r => r.Id == id);
        }
    }
}
