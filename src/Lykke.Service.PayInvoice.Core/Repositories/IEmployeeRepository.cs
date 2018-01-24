using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IReadOnlyList<IEmployee>> GetAsync(string merchantId);

        Task<IEmployee> GetAsync(string merchantId, string employeeId);
        
        Task<IEmployee> FindAsync(string email);
        
        Task<IEmployee> InsertAsync(IEmployee employee);
        
        Task MergeAsync(IEmployee employee);

        Task DeleteAsync(string merchantId, string employeeId);
    }
}