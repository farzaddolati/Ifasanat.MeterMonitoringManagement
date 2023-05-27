using Ifasanat.MeterMonitoringManagement.Domain;

namespace Ifasanat.MeterMonitoringManagement.Repositories
{
    public interface ICityRepository
    {
        Task<City> GetByIdAsync(int id);
        Task<IEnumerable<City>> GetAllAsync();
        Task AddAsync(City city);
        Task UpdateAsync(City city);
        Task DeleteAsync(City city);
    }
}
