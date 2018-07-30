using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Extensions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Extensions;
using Lykke.Service.PayInvoice.Models.Employee;
using Lykke.Service.PayInvoice.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILog _log;

        public EmployeesController(
            IEmployeeService employeeService,
            ILogFactory logFactory)
        {
            _employeeService = employeeService;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Returns employees.
        /// </summary>
        /// <returns>The collection of employees.</returns>
        /// <response code="200">The collection of employees.</response>
        [HttpGet]
        [SwaggerOperation("EmployeesGetAll")]
        [ProducesResponseType(typeof(List<EmployeeModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            IEnumerable<Employee> employees = await _employeeService.GetAllAsync();

            var model = Mapper.Map<List<EmployeeModel>>(employees);

            return Ok(model);
        }

        /// <summary>
        /// Returns an employee by id.
        /// </summary>
        /// <param name="employeeId">The employee id.</param>
        /// <returns>The employee.</returns>
        /// <response code="200">The employee.</response>
        /// <response code="404">Employee not found.</response>
        [HttpGet]
        [Route("{employeeId}")]
        [SwaggerOperation("EmployeesGetById")]
        [ProducesResponseType(typeof(EmployeeModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByIdAsync(string employeeId)
        {
            Employee employee = await _employeeService.GetByIdAsync(employeeId);

            if (employee == null)
                return NotFound();

            var model = Mapper.Map<EmployeeModel>(employee);

            return Ok(model);
        }

        /// <summary>
        /// Returns an employee by email
        /// </summary>
        /// <param name="email">The employee email</param>
        /// <returns>The employee</returns>
        /// <response code="200">The employee.</response>
        /// <response code="404">Employee not found.</response>
        /// <response code="400">Email has invalid value</response>
        [HttpGet]
        [Route("byEmail/{email}")]
        [SwaggerOperation("EmployeesGetByEmail")]
        [ProducesResponseType(typeof(EmployeeModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByEmail(string email)
        {
            email = Uri.UnescapeDataString(email);

            if (!email.IsValidEmailAndRowKey())
                return BadRequest(ErrorResponse.Create("Email value is invalid"));

            Employee employee = await _employeeService.GetByEmailAsync(email);

            if (employee == null)
                return NotFound(ErrorResponse.Create("Employee not found"));

            return Ok(Mapper.Map<EmployeeModel>(employee));
        }

        /// <summary>
        /// Creates an employee.
        /// </summary>
        /// <param name="model">The employee info.</param>
        /// <response code="200">The employee successfully created.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [SwaggerOperation("EmployeesAdd")]
        [ValidateModel]
        [ProducesResponseType(typeof(EmployeeModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddAsync([FromBody] CreateEmployeeModel model)
        {
            try
            {
                var employee = Mapper.Map<Employee>(model);

                Employee createdEmployee = await _employeeService.AddAsync(employee);

                var employeeModel = Mapper.Map<EmployeeModel>(createdEmployee);

                return Ok(employeeModel);
            }
            catch (EmployeeExistException ex)
            {
                _log.WarningWithDetails(ex.Message, model.Sanitize());

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Updates an employee.
        /// </summary>
        /// <param name="model">The employee info.</param>
        /// <response code="200">The employee successfully updated.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPut]
        [SwaggerOperation("EmployeesUpdate")]
        [ValidateModel]
        [ProducesResponseType(typeof(EmployeeModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateEmployeeModel model)
        {
            try
            {
                var employee = Mapper.Map<Employee>(model);

                await _employeeService.UpdateAsync(employee);

                return NoContent();
            }
            catch (EmployeeNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, model.Sanitize());

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Deletes an employee.
        /// </summary>
        /// <param name="employeeId">The employee id.</param>
        /// <response code="204">Invoice successfully deleted.</response>
        [HttpDelete]
        [Route("{employeeId}")]
        [SwaggerOperation("EmployeesDelete")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteAsync(string employeeId)
        {
            await _employeeService.DeleteAsync(employeeId);

            return NoContent();
        }
    }
}
