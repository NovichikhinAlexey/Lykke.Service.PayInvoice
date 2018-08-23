using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IReadOnlyList<Employee>> GetAsync();

        Task<Employee> GetAsync(string employeeId, string merchantId);

        Task<Employee> GetByIdAsync(string employeeId);

        Task<IReadOnlyList<Employee>> GetByMerchantIdAsync(string merchantId);
        
        Task<Employee> FindAsync(string email);
        
        Task<Employee> InsertAsync(Employee employee);
        
        Task UpdateAsync(Employee employee, string previousEmail);

        Task DeleteAsync(string employeeId);
    }
}
