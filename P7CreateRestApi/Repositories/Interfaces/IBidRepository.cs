using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories.Interfaces
{
    public interface IBidRepository
    {
        Task<IEnumerable<Bid>> GetAllAsync();
        Task<Bid?> GetByIdAsync(int id);
        Task AddAsync(Bid bid);
        Task UpdateAsync(Bid bid);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
