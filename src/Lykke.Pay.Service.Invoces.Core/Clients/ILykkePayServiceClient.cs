using System.Threading.Tasks;

namespace Lykke.Pay.Service.Invoces.Core.Clients
{
    public interface ILykkePayServiceClient
    {
        Task<OrderResponse> CreateOrder(OrderRequest orderRequest, string merchantId);

        Task<OrderResponse> ReCreateOrder(string address, string merchantId);
    }
}
