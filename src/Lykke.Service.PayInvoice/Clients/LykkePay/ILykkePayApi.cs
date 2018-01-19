using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Clients;
using Refit;

namespace Lykke.Service.PayInvoice.Clients.LykkePay
{
    public interface ILykkePayApi
    {
        [Post("/api/v1/Order")]
        Task<OrderResponse> CreateOrder([Body] OrderRequest orderRequest, [Header("Lykke-Merchant-Id")] string merchantId, [Header("Lykke-Merchant-Traster-SignIn")] string trastedKey);

        [Post("/api/v1/Order/ReCreate/{address}")]
        [Headers("Lykke-Merchant-Traster-SignIn: true")]
        Task<OrderResponse> ReCreateOrder(string address, [Header("Lykke-Merchant-Id")] string merchantId, [Header("Lykke-Merchant-Traster-SignIn")] string trastedKey);
    }
}
