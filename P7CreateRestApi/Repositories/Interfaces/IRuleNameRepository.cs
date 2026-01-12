using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories.Interfaces
{
    public interface IRuleNameRepository
    {
        Task<IEnumerable<RuleName>> GetAllAsync();
        Task<RuleName?> GetByIdAsync(int id);
        Task AddAsync(RuleName ruleName);
        Task UpdateAsync(RuleName ruleName);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
