using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.PayInvoice.Core.Domain.DataMigrations;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Extensions;
using Lykke.Service.PayInvoice.Models.DataMigration;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class DataMigrationsController : Controller
    {
        private readonly IDataMigrationService _dataMigrationService;
        private readonly ILog _log;

        public DataMigrationsController(
            IDataMigrationService dataMigrationService,
            ILog log)
        {
            _dataMigrationService = dataMigrationService;
            _log = log.CreateComponentScope(nameof(DataMigrationsController));
        }

        /// <summary>
        /// Get all executed migrations
        /// </summary>
        [HttpGet]
        [Route("getall")]
        [SwaggerOperation("DataMigrationsGetAll")]
        [ProducesResponseType(typeof(IReadOnlyList<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            IReadOnlyList<string> migrations = await _dataMigrationService.GetAll();

            return Ok(migrations);
        }

        /// <summary>
        /// Executes migration
        /// </summary>
        /// <param name="migrationName">Migration name</param>
        /// <returns></returns>
        [HttpPost]
        [Route("execute")]
        [SwaggerOperation("DataMigrationsExecute")]
        [ProducesResponseType(typeof(DataMigrationResult), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ExecuteAsync([FromBody] DataMigrationExecuteModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                var result = await _dataMigrationService.ExecuteAsync(model.MigrationName);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _log.WriteError(nameof(ExecuteAsync), model, exception);
                throw;
            }
        }
    }
}