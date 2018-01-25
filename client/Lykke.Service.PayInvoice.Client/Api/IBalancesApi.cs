using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Balances;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IBalancesApi
    {
        [Get("/api/merchants/{merchantId}/balances/{assetId}")]
        Task<BalanceModel> GetAsync(string merchantId, string assetId);
    }
}