using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Exceptions;
using Lykke.Service.PayInternal.Client.Models.MerchantGroups;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Services;

namespace Lykke.Service.PayInvoice.Services
{
    public class MerchantService : IMerchantService
    {
        private readonly IPayInternalClient _payInternalClient;

        public MerchantService(
            IPayInternalClient payInternalClient)
        {
            _payInternalClient = payInternalClient;
        }

        public async Task<IReadOnlyList<string>> GetGroupMerchants(string merchantId)
        {
            MerchantsByUsageResponse response;

            try
            {
                response = await _payInternalClient.GetMerchantsByUsageAsync(
                new GetMerchantsByUsageRequest
                {
                    MerchantId = merchantId,
                    MerchantGroupUse = MerchantGroupUse.Billing
                });
            }
            catch (DefaultErrorResponseException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new MerchantGroupNotFoundException(merchantId);
            }

            return response.Merchants.ToList();
        }
    }
}
