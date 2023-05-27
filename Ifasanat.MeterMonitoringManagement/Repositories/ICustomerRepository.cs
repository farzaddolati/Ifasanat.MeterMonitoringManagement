using Ifasanat.MeterMonitoringManagement.Domain;

namespace Ifasanat.MeterMonitoringManagement.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(int id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
    }
}
