using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IReadOnlyList<Employee>> GetAsync(string merchantId);

        Task<Employee> GetAsync(string merchantId, string employeeId);
        
        Task<Employee> FindAsync(string email);
        
        Task<Employee> InsertAsync(Employee employee);
        
        Task UpdateAsync(Employee employee);

        Task DeleteAsync(string merchantId, string employeeId);
    }
}