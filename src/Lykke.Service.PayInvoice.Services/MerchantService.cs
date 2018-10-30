using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Cache;
using Lykke.Service.PayInternal.Client.Exceptions;
using Lykke.Service.PayMerchant.Client.Models;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Settings;
using Lykke.Service.PayMerchant.Client;
using Microsoft.Extensions.Caching.Memory;

namespace Lykke.Service.PayInvoice.Services
{
    public class MerchantService : IMerchantService
    {
        private readonly CacheExpirationPeriodsSettings _cacheExpirationPeriods;
        private readonly OnDemandDataCache<string> _merchantNamesCache;
        private readonly IPayMerchantClient _payMerchantClient;

        public MerchantService(
            IMemoryCache memoryCache,
            CacheExpirationPeriodsSettings cacheExpirationPeriods, 
            IPayMerchantClient payMerchantClient)
        {
            _cacheExpirationPeriods = cacheExpirationPeriods;
            _payMerchantClient = payMerchantClient;
            _merchantNamesCache = new OnDemandDataCache<string>(memoryCache);
        }

        public async Task<IReadOnlyList<string>> GetGroupMerchantsAsync(string merchantId)
        {
            MerchantsByUsageResponse response;

            try
            {
                response = await _payMerchantClient.GroupsApi.GetMerchantsByUsageAsync(
                new GetMerchantsByUsageRequest
                {
                    MerchantId = merchantId,
                    MerchantGroupUse = MerchantGroupUse.Billing
                });
            }
            catch (ClientApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
            {
                throw new MerchantGroupNotFoundException(merchantId);
            }

            return response.Merchants.ToList();
        }

        public async Task<string> GetMerchantNameAsync(string merchantId)
        {
            var merchantName = await _merchantNamesCache.GetOrAddAsync
                (
                    $"MerchantName-{merchantId}",
                    async x => {
                        var merchant = await _payMerchantClient.Api.GetByIdAsync(merchantId);
                        return merchant.DisplayName;
                    },
                    _cacheExpirationPeriods.MerchantName
                );

            return merchantName;
        }
    }
}
