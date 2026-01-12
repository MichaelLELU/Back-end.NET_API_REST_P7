using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories.Interfaces
{
    public interface ITradeRepository
    {
        Task<IEnumerable<Trade>> GetAllAsync();
        Task<Trade?> GetByIdAsync(int id);
        Task AddAsync(Trade trade);
        Task UpdateAsync(Trade trade);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
