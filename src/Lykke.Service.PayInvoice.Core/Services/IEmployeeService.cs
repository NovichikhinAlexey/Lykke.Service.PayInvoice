using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IEmployeeService
    {
        Task<IReadOnlyList<IEmployee>> GetAsync(string merchantId);

        Task<IEmployee> GetAsync(string merchantId, string employeeId);
        
        Task<IEmployee> AddAsync(IEmployee employee);
        
        Task UpdateAsync(IEmployee employee);

        Task DeleteAsync(string merchantId, string employeeId);
    }
}