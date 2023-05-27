using Ifasanat.MeterMonitoringManagement.Domain;
using Ifasanat.MeterMonitoringManagement.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ifasanat.MeterMonitoringManagement.Data
{
    public class CityRepository : ICityRepository
    {
        private readonly MyApiDbContext _dbContext;

        public CityRepository(MyApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<City> GetByIdAsync(int id)
        {
            return await _dbContext.Cities.FindAsync(id);
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _dbContext.Cities.ToListAsync();
        }

        public async Task AddAsync(City city)
        {
            await _dbContext.Cities.AddAsync(city);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(City city)
        {
            _dbContext.Entry(city).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(City city)
        {
            _dbContext.Cities.Remove(city);
            await _dbContext.SaveChangesAsync();
        }
    }
}
