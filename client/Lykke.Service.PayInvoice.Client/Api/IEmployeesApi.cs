using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Employee;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IEmployeesApi
    {
        [Get("/api/Employees")]
        Task<IReadOnlyList<EmployeeModel>> GetAllAsync();

        [Get("/api/Employees/{employeeId}")]
        Task<EmployeeModel> GetByIdAsync(string employeeId);

        [Post("/api/Employees")]
        Task<EmployeeModel> AddAsync([Body]CreateEmployeeModel model);

        [Put("/api/Employees")]
        Task UpdateAsync([Body] UpdateEmployeeModel model);

        [Delete("/api/Employees")]
        Task DeleteAsync(string employeeId);
    }
}