using System;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Clients;
using Microsoft.Extensions.PlatformAbstractions;
using Refit;

namespace Lykke.Service.PayInvoice.Clients.LykkePay
{
    public class LykkePayServiceClient : IDisposable, ILykkePayServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILykkePayApi _api;
        private readonly LykkePayServiceClientSettings _settings;

        public LykkePayServiceClient(LykkePayServiceClientSettings settings)
        {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));

            if(string.IsNullOrEmpty(settings.ServiceUrl))
                throw new ArgumentException("Service URL Required");

            _settings = settings;

            _httpClient = new HttpClient //TODO Make the httpClient like singletone
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

        public async Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest, string merchantId)
        {
            return await RunAsync(() => _api.CreateOrder(orderRequest, merchantId, _settings.LykkePayTrastedConnectionKey));
        }

        public async Task<OrderResponse> ReCreateOrderAsync(string address, string merchantId)
        {
            return await RunAsync(() => _api.ReCreateOrder(address, merchantId, _settings.LykkePayTrastedConnectionKey));
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
