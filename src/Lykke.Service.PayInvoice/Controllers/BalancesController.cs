using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.Balances.Client.ResponseModels;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Models.Merchant;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Models.Balances;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api")]
    public class BalancesController : Controller
    {
        private readonly IBalancesClient _balancesClient;
        private readonly IPayInternalClient _payInternalClient;
        private readonly ILog _log;

        public BalancesController(
            IBalancesClient balancesClient,
            IPayInternalClient payInternalClient,
            ILog log)
        {
            _balancesClient = balancesClient;
            _payInternalClient = payInternalClient;
            _log = log;
        }

        /// <summary>
        /// Returns merchant asset balance.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="assetId">The asset id.</param>
        /// <returns>The merchant asset balance.</returns>
        /// <response code="200">The merchant asset balance.</response>
        [HttpGet]
        [Route("merchants/{merchantId}/balances/{assetId}")]
        [SwaggerOperation("BalancesGetMerchantBalance")]
        [ProducesResponseType(typeof(List<BalanceModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(string merchantId, string assetId)
        {
            try
            {
                var model = new BalanceModel { AssetId = assetId };
                
                MerchantModel merchant = await _payInternalClient.GetMerchantByIdAsync(merchantId);
                
                if(merchant?.LwId == null)
                    return Ok(model);

                ClientBalanceModel clientBalance = await _balancesClient.GetClientBalanceByAssetId(
                    new ClientBalanceByAssetIdModel
                    {
                        AssetId = assetId,
                        ClientId = merchant.LwId
                    });

                model.Balance = clientBalance?.Balance;
                model.Reserved = clientBalance?.Reserved;

                return Ok(model);
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(BalancesController), nameof(GetAsync),
                    merchantId.ToContext(nameof(assetId))
                        .ToContext(nameof(merchantId), merchantId)
                        .ToJson(), exception);
                
                throw;
            }
        }
    }
}