using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Extensions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Extensions;
using Lykke.Service.PayInvoice.Models.MerchantSetting;
using LykkePay.Common.Validation;
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
            ILogFactory logFactory)
        {
            _merchantSettingService = merchantSettingService;
            _log = logFactory.CreateLog(this);
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
        [ValidateModel]
        public async Task<IActionResult> GetByIdAsync([Required][RowKey] string merchantId)
        {
            try
            {
                MerchantSetting merchantSettings = await _merchantSettingService.GetByIdAsync(merchantId);

                return Ok(merchantSettings);
            }
            catch (MerchantSettingNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, new { merchantId });

                return NotFound(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Create or update merchant setting
        /// </summary>
        /// <param name="model">The merchant setting info</param>
        /// <response code="200">Successfully created</response>
        /// <response code="404">Merchant not found</response>
        /// <response code="400">Invalid model</response>
        [HttpPost]
        [SwaggerOperation("MerchantSettingSet")]
        [ProducesResponseType(typeof(MerchantSetting), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> SetAsync([FromBody] SetMerchantSettingModel model)
        {
            try
            {
                var merchantSettingModel = Mapper.Map<MerchantSetting>(model);

                MerchantSetting merchantSetting = await _merchantSettingService.SetAsync(merchantSettingModel);

                return Ok(merchantSetting);
            }
            catch (MerchantNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, new { ex.MerchantId });

                return NotFound(ErrorResponse.Create(ex.Message));
            }
            catch (AssetNotAvailableForMerchantException ex)
            {
                _log.WarningWithDetails(ex.Message, model);

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
        [ValidateModel]
        public async Task<IActionResult> GetBaseAssetByIdAsync([Required][RowKey] string merchantId)
        {
            try
            {
                string baseAsset = await _merchantSettingService.GetBaseAssetByIdAsync(merchantId);

                return Ok(baseAsset);
            }
            catch (MerchantSettingNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, new { merchantId });

                return NotFound(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Update base asset
        /// </summary>
        /// <param name="model">Update base asset request</param>
        /// <response code="200">Successfully updated</response>
        /// <response code="404">Merchant setting is not found</response>
        /// <response code="400">Invalid model</response>
        [HttpPost("baseAsset")]
        [SwaggerOperation("SetBaseAsset")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> SetBaseAssetAsync([FromBody] UpdateBaseAssetRequest model)
        {
            try
            {
                MerchantSetting merchantSettings = await _merchantSettingService.GetByIdAsync(model.MerchantId);

                merchantSettings.BaseAsset = model.BaseAsset;

                await _merchantSettingService.SetAsync(merchantSettings);

                return Ok();
            }
            catch (MerchantSettingNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, model);

                return NotFound(ErrorResponse.Create(ex.Message));
            }
            catch (AssetNotAvailableForMerchantException ex)
            {
                _log.WarningWithDetails(ex.Message, model);

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }
    }
}
