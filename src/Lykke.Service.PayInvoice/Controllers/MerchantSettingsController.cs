using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Extensions;
using Lykke.Service.PayInvoice.Models.MerchantSetting;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class MerchantSettingsController : Controller
    {
        private readonly IMerchantSettingService _merchantSettingService;
        private readonly ILog _log;

        public MerchantSettingsController(
            IMerchantSettingService merchantSettingService,
            ILog log)
        {
            _merchantSettingService = merchantSettingService;
            _log = log.CreateComponentScope(nameof(MerchantSettingsController));
        }

        /// <summary>
        /// Returns merchant setting by id
        /// </summary>
        /// <param name="merchantId">The merchant id</param>
        /// <response code="200">The merchant setting</response>
        /// <response code="404">Merchant setting not found</response>
        [HttpGet]
        [Route("{merchantId}")]
        [SwaggerOperation("MerchantSettingGetById")]
        [ProducesResponseType(typeof(MerchantSetting), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByIdAsync(string merchantId)
        {
            MerchantSetting merchantSetting = await _merchantSettingService.GetByIdAsync(merchantId);

            if (merchantSetting == null)
                return NotFound();

            return Ok(merchantSetting);
        }

        /// <summary>
        /// Create or update merchant setting
        /// </summary>
        /// <param name="model">The merchant setting info</param>
        /// <response code="200">Successfully created</response>
        /// <response code="400">Invalid model</response>
        [HttpPost]
        [SwaggerOperation("MerchantSettingSet")]
        [ProducesResponseType(typeof(MerchantSetting), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetAsync([FromBody] SetMerchantSettingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                var merchantSettingModel = Mapper.Map<MerchantSetting>(model);

                MerchantSetting merchantSetting = await _merchantSettingService.SetAsync(merchantSettingModel);

                return Ok(merchantSetting);
            }
            catch (Exception exception)
            {
                _log.WriteError(nameof(SetAsync), model, exception);
                throw;
            }
        }
    }
}