using System;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Core.Clients;
using Microsoft.Extensions.PlatformAbstractions;
using Refit;

namespace Lykke.Pay.Service.Invoces.Clients.LykkePay
{
    public class LykkePayServiceClient : IDisposable, ILykkePayServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILykkePayApi _api;

        public LykkePayServiceClient(LykkePayServiceClientSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));

            if(string.IsNullOrEmpty(settings.ServiceUrl))
                throw new ArgumentException("Service URL Required");

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(settings.ServiceUrl),
                DefaultRequestHeaders =
                {
                    {
                        "User-Agent",
                        $"{PlatformServices.Default.Application.ApplicationName}/{PlatformServices.Default.Application.ApplicationVersion}"
                    }
                }
            };

            _api = RestService.For<ILykkePayApi>(_httpClient);
        }

        public async Task<OrderResponse> CreateOrder(OrderRequest orderRequest, string merchantId)
        {
            return await RunAsync(() => _api.CreateOrder(orderRequest, merchantId));
        }

        public async Task<OrderResponse> ReCreateOrder(string address, string merchantId)
        {
            return await RunAsync(() => _api.ReCreateOrder(address, merchantId));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private async Task<T> RunAsync<T>(Func<Task<T>> method)
        {
            try
            {
                return await method();
            }
            catch (ApiException exception)
            {
                throw new ErrorResponseException("An error occurred  during calling api", exception);
            }
        }
    }
}
