using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Employee;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IEmployeesApi
    {
        [Get("/api/merchants/{merchantId}/employees")]
        Task<IReadOnlyList<EmployeeModel>> GetAsync(string merchantId);

        [Get("/api/merchants/{merchantId}/employees/{employeeId}")]
        Task<EmployeeModel> GetAsync(string merchantId, string employeeId);
        
        [Post("/api/merchants/{merchantId}/employees")]
        Task<EmployeeModel> AddAsync(string merchantId, [Body]CreateEmployeeModel model);

        [Put("/api/merchants/{merchantId}/employees/{employeeId}")]
        Task UpdateAsync(string merchantId, string employeeId, [Body] CreateEmployeeModel model);

        [Delete("/api/merchants/{merchantId}/employees/{employeeId}")]
        Task DeleteAsync(string merchantId, string employeeId);
    }
}