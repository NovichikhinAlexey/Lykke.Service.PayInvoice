using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IMerchantsApi
    {
        [Get("/api/merchants/{merchantId}/assets/settlement")]
        Task<IReadOnlyList<string>> GetSettlementAssetsAsync(string merchantId);

        [Get("/api/merchants/{merchantId}/assets/payment")]
        Task<IReadOnlyList<string>> GetPaymentAssetsAsync(string merchantId);
    }
}
