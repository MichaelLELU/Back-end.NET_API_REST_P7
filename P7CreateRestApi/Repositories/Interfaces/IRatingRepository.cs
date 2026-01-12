using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories.Interfaces
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetAllAsync();
        Task<Rating?> GetByIdAsync(int id);
        Task AddAsync(Rating rating);
        Task UpdateAsync(Rating rating);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
