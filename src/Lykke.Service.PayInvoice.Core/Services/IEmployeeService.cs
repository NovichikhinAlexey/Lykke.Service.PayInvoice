using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IEmployeeService
    {
        Task<IReadOnlyList<Employee>> GetAsync(string merchantId);

        Task<Employee> GetAsync(string merchantId, string employeeId);
        
        Task<Employee> AddAsync(Employee employee);
        
        Task UpdateAsync(Employee employee);

        Task DeleteAsync(string merchantId, string employeeId);
    }
}