using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Extensions;
using Lykke.Service.PayInvoice.Models.Employee;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILog _log;

        public EmployeesController(
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
        [Route("merchants/{merchantId}/employees")]
        [SwaggerOperation("EmployeesGetByMerchantId")]
        [ProducesResponseType(typeof(List<EmployeeModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(string merchantId)
        {
            IEnumerable<IEmployee> employees = await _employeeService.GetAsync(merchantId);

            var model = Mapper.Map<List<EmployeeModel>>(employees);

            return Ok(model);
        }
        
        /// <summary>
        /// Returns an employee.
        /// </summary>
        /// <returns>The employee.</returns>
        /// <response code="200">The employee.</response>
        /// <response code="404">Employee not found.</response>
        [HttpGet]
        [Route("merchants/{merchantId}/employees/{employeeId}")]
        [SwaggerOperation("EmployeesGetByMerchantIdById")]
        [ProducesResponseType(typeof(EmployeeModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync(string merchantId, string employeeId)
        {
            try
            {
                IEmployee employee = await _employeeService.GetAsync(merchantId, employeeId);

                var model = Mapper.Map<EmployeeModel>(employee);
                
                return Ok(model);
            }
            catch (EmployeeNotFoundException exception)
            {
                await _log.WriteWarningAsync(nameof(EmployeesController), nameof(GetAsync),
                    merchantId.ToContext(nameof(merchantId))
                        .ToContext(employeeId, nameof(employeeId))
                        .ToJson(), exception.Message);
                
                return NotFound(ErrorResponse.Create(exception.Message));
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(EmployeesController), nameof(GetAsync),
                    merchantId.ToContext(nameof(merchantId))
                        .ToContext(employeeId, nameof(employeeId))
                        .ToJson(), exception);

                throw;
            }
        }

        /// <summary>
        /// Creates an employee.
        /// </summary>
        /// <param name="merchantId">The merchan id.</param>
        /// <param name="model">The employee info.</param>
        /// <response code="200">The employee credentials successfully created.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [Route("merchants/{merchantId}/employees")]
        [SwaggerOperation("EmployeesAdd")]
        [ProducesResponseType(typeof(EmployeeModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddAsync(string merchantId, [FromBody]CreateEmployeeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                var employee = Mapper.Map<Employee>(model);
                employee.MerchantId = merchantId;

                IEmployee createdEmployee = await _employeeService.AddAsync(employee);

                var employeeModel = Mapper.Map<EmployeeModel>(createdEmployee);
                
                return Ok(employeeModel);
            }
            catch (EmployeeExistException exception)
            {
                await _log.WriteWarningAsync(nameof(EmployeesController), nameof(AddAsync),
                    model.ToContext()
                        .ToContext(nameof(merchantId), merchantId)
                        .ToJson(), exception.Message);
                
                return BadRequest(ErrorResponse.Create(exception.Message));
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(EmployeesController), nameof(AddAsync),
                    model.ToContext()
                        .ToContext(nameof(merchantId), merchantId)
                        .ToJson(), exception);

                throw;
            }
        }
        
        /// <summary>
        /// Updates an employee.
        /// </summary>
        /// <param name="merchantId">The merchan id.</param>
        /// <param name="employeeId">The employee id.</param>
        /// <param name="model">The employee info.</param>
        /// <response code="200">The employee credentials successfully created.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPut]
        [Route("merchants/{merchantId}/employees/{employeeId}")]
        [SwaggerOperation("EmployeesUpdate")]
        [ProducesResponseType(typeof(EmployeeModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateAsync(string merchantId, string employeeId, [FromBody]CreateEmployeeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                var employee = Mapper.Map<Employee>(model);
                employee.MerchantId = merchantId;
                employee.Id = employeeId;

                await _employeeService.UpdateAsync(employee);
                
                return NoContent();
            }
            catch (EmployeeNotFoundException exception)
            {
                await _log.WriteWarningAsync(nameof(EmployeesController), nameof(UpdateAsync),
                    model.ToContext()
                        .ToContext(nameof(merchantId), merchantId)
                        .ToJson(), exception.Message);
                
                return BadRequest(ErrorResponse.Create(exception.Message));
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(EmployeesController), nameof(UpdateAsync),
                    model.ToContext()
                        .ToContext(nameof(merchantId), merchantId)
                        .ToContext(nameof(employeeId), employeeId)
                        .ToJson(), exception);

                throw;
            }
        }

        /// <summary>
        /// Deletes an employee.
        /// </summary>
        /// <param name="merchantId">The merchan id.</param>
        /// <param name="employeeId">The employee id.</param>
        /// <response code="204">Invoice successfully deleted.</response>
        [HttpDelete]
        [Route("merchants/{merchantId}/employees/{employeeId}")]
        [SwaggerOperation("EmployeesDelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteAsync(string merchantId, string employeeId)
        {
            await _employeeService.DeleteAsync(merchantId, employeeId);
            
            return NoContent();
        }
    }
}