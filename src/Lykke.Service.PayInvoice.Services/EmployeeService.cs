using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Services.Extensions;

namespace Lykke.Service.PayInvoice.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILog _log;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            ILog log)
        {
            _employeeRepository = employeeRepository;
            _log = log;
        }
        
        public async Task<IReadOnlyList<Employee>> GetAsync(string merchantId)
        {
            return await _employeeRepository.GetAsync(merchantId);
        }

        public async Task<Employee> GetAsync(string merchantId, string employeeId)
        {
            return await _employeeRepository.GetAsync(merchantId, employeeId);
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            Employee existingEmployee = await _employeeRepository.FindAsync(employee.Email);
            
            if(existingEmployee != null)
                throw new EmployeeExistException();
                
            Employee createdEmployee = await _employeeRepository.InsertAsync(employee);

            await _log.WriteInfoAsync(nameof(EmployeeService), nameof(AddAsync),
                employee.ToContext().ToJson(), "Employee added.");
            
            return createdEmployee;
        }

        public async Task UpdateAsync(Employee employee)
        {
            Employee existingEmployee = await _employeeRepository.GetAsync(employee.MerchantId, employee.Id);
            
            if(existingEmployee == null)
                throw new EmployeeNotFoundException();
                
            await _employeeRepository.UpdateAsync(employee);
            
            await _log.WriteInfoAsync(nameof(EmployeeService), nameof(AddAsync),
                employee.ToContext().ToJson(), "Employee updated.");
        }

        public async Task DeleteAsync(string merchantId, string employeeId)
        {
            await _employeeRepository.DeleteAsync(merchantId, employeeId);
            
            await _log.WriteInfoAsync(nameof(EmployeeService), nameof(AddAsync),
                merchantId.ToContext(nameof(merchantId))
                    .ToContext(nameof(employeeId), employeeId)
                    .ToJson(), "Employee deleted.");
        }
    }
}