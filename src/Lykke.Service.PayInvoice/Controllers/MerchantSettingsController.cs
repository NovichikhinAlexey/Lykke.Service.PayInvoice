using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
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
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetByIdAsync(string merchantId)
        {
            try
            {
                MerchantSetting merchantSettings = await _merchantSettingService.GetByIdAsync(merchantId);

                return Ok(merchantSettings);
            }
            catch (MerchantSettingNotFoundException ex)
            {
                return NotFound(ErrorResponse.Create(ex.Message));
            }
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
            catch (AssetNotAvailableForMerchantException ex)
            {
                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Returns base asset by id
        /// </summary>
        /// <param name="merchantId">The merchant id</param>
        /// <response code="200">The base asset</response>
        /// <response code="404">Merchant setting is not found</response>
        [HttpGet]
        [Route("baseAsset/{merchantId}")]
        [SwaggerOperation("GetBaseAssetById")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetBaseAssetByIdAsync(string merchantId)
        {
            try
            {
                string baseAsset = await _merchantSettingService.GetBaseAssetByIdAsync(merchantId);

                return Ok(baseAsset);
            }
            catch (MerchantSettingNotFoundException ex)
            {
                return NotFound(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Create or update base asset
        /// </summary>
        /// <response code="200">Successfully created or updated</response>
        /// <response code="404">Merchant setting is not found</response>
        /// <response code="400">Invalid model</response>
        [HttpPost("baseAsset")]
        [SwaggerOperation("SetBaseAsset")]
        [ProducesResponseType(typeof(MerchantSetting), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetBaseAssetAsync([FromBody] UpdateBaseAssetRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                await _merchantSettingService.SetBaseAssetAsync(model.MerchantId, model.BaseAsset);

                return Ok();
            }
            
            catch (AssetNotAvailableForMerchantException ex)
            {
                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }
    }
}
