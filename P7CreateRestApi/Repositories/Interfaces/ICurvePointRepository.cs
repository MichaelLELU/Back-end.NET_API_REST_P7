using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories.Interfaces
{
    public interface ICurvePointRepository
    {
        Task<IEnumerable<CurvePoint>> GetAllAsync();
        Task<CurvePoint?> GetByIdAsync(int id);
        Task AddAsync(CurvePoint curvePoint);
        Task UpdateAsync(CurvePoint curvePoint);
        Task<bool> DeleteAsync(int id);
    }
}
