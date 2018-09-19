using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IEmployeeService
    {
        Task<IReadOnlyList<Employee>> GetAllAsync();

        Task<Employee> GetByIdAsync(string employeeId);

        Task<Employee> GetByEmailAsync(string email);

        Task<IReadOnlyList<Employee>> GetByMerchantIdAsync(string merchantId);
        
        Task<Employee> AddAsync(Employee employee);

        Task MarkDeletedAsync(string merchantId, string employeeId);
        
        Task UpdateAsync(Employee employee);

        Task DeleteAsync(string employeeId);
    }
}
