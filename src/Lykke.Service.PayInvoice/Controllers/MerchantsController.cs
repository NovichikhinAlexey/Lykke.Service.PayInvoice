using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Models.Asset;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class MerchantsController : Controller
    {
        private readonly IPayInternalClient _payInternalClient;
        private readonly ILog _log;

        public MerchantsController(IPayInternalClient payInternalClient, ILog log)
        {
            _payInternalClient = payInternalClient;
            _log = log;
        }

        /// <summary>
        /// Returns a collection of assets allowed for settlement.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <returns>A collection of assets.</returns>
        /// <response code="200">A collection of assets.</response>
        [HttpGet]
        [Route("{merchantId}/assets/settlement")]
        [SwaggerOperation("MerchantsGetSettlementAssets")]
        [ProducesResponseType(typeof(List<string>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetSettlementAssetsAsync(string merchantId)
        {
            try
            {
                var response =
                    await _payInternalClient.ResolveAvailableAssetsAsync(merchantId, AssetAvailabilityType.Settlement);

                return Ok(response.Assets.ToList());
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(MerchantsController), nameof(GetSettlementAssetsAsync),
                    new {merchantId}.ToJson(), exception);

                throw;
            }
        }

        /// <summary>
        /// Returns a collection of assets allowed for payment.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <returns>A collection of assets.</returns>
        /// <response code="200">A collection of assets.</response>
        [HttpGet]
        [Route("{merchantId}/assets/payment")]
        [SwaggerOperation("MerchantsGetPaymentAssets")]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPaymentAssetsAsync(string merchantId)
        {
            try
            {
                var response =
                    await _payInternalClient.ResolveAvailableAssetsAsync(merchantId, AssetAvailabilityType.Payment);

                return Ok(response.Assets.ToList());
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(MerchantsController), nameof(GetPaymentAssetsAsync),
                    new { merchantId }.ToJson(), exception);

                throw;
            }
        }
    }
}
