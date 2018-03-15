using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Models.Asset;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class AssetsController : Controller
    {
        private readonly IPayInternalClient _payInternalClient;

        public AssetsController(IPayInternalClient payInternalClient)
        {
            _payInternalClient = payInternalClient;
        }

        /// <summary>
        /// Returns a collection of assets allowed for settlement.
        /// </summary>
        /// <returns>A collection of assets.</returns>
        /// <response code="200">A collection of assets.</response>
        [HttpGet]
        [Route("settlement")]
        [SwaggerOperation("AssetsGetSettlement")]
        [ProducesResponseType(typeof(List<AssetModel>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetSettlementAsync()
        {
            AvailableAssetsResponse response =
                await _payInternalClient.GetAvailableAsync(AssetAvailabilityType.Settlement);

            List<AssetModel> model = response.Assets
                .Select(o => new AssetModel
                {
                    Id = o,
                    Name = o
                })
                .ToList();

            return Ok(model);
        }

        /// <summary>
        /// Returns a collection of assets allowed for payment.
        /// </summary>
        /// <returns>A collection of assets.</returns>
        /// <response code="200">A collection of assets.</response>
        [HttpGet]
        [Route("payment")]
        [SwaggerOperation("AssetsGetPayment")]
        [ProducesResponseType(typeof(List<AssetModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPaymentAsync()
        {
            AvailableAssetsResponse response =
                await _payInternalClient.GetAvailableAsync(AssetAvailabilityType.Payment);

            List<AssetModel> model = response.Assets
                .Select(o => new AssetModel
                {
                    Id = o,
                    Name = o
                })
                .ToList();

            return Ok(model);
        }
    }
}
