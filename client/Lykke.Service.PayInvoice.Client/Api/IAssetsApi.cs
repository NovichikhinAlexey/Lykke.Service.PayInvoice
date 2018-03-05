using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Assets;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IAssetsApi
    {
        [Get("/api/assets/settlement")]
        Task<IReadOnlyList<AssetModel>> GetSettlementAsync();

        [Get("/api/assets/payment")]
        Task<IReadOnlyList<AssetModel>> GetPaymentAsync();
    }
}
