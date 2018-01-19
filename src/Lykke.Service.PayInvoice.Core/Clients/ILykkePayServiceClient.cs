using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Clients
{
    public interface ILykkePayServiceClient
    {
        Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest, string merchantId);

        Task<OrderResponse> ReCreateOrderAsync(string address, string merchantId);
    }
}
