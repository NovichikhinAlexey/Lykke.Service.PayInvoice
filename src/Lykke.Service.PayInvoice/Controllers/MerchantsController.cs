using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Models.Employee;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class MerchantsController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILog _log;

        public MerchantsController(
            IEmployeeService employeeService,
            ILog log)
        {
            _employeeService = employeeService;
            _log = log;
        }

        /// <summary>
        /// Returns merchant employees.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <returns>The collection of employees.</returns>
        /// <response code="200">The collection of employees.</response>
        [HttpGet]
        [Route("{merchantId}/employees")]
        [SwaggerOperation("MerchantsGetEmployees")]
        [ProducesResponseType(typeof(List<EmployeeModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetEmployeesAsync(string merchantId)
        {
            IEnumerable<Employee> employees = await _employeeService.GetByMerchantIdAsync(merchantId);

            var model = Mapper.Map<List<EmployeeModel>>(employees);

            return Ok(model);
        }
    }
}
