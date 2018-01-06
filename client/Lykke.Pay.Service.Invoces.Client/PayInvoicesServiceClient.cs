using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Client.Api;
using Lykke.Pay.Service.Invoces.Client.Models.Invoice;
using Microsoft.Extensions.PlatformAbstractions;
using Refit;

namespace Lykke.Pay.Service.Invoces.Client
{
    public class PayInvoicesServiceClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IInvoiceApi _api;
        private readonly ApiRunner _runner;

        public PayInvoicesServiceClient(PayInvoicesServiceClientSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrEmpty(settings.ServiceUrl))
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

            _api = RestService.For<IInvoiceApi>(_httpClient);
            _runner = new ApiRunner();
        }

        public async Task<InvoiceModel> GetAsync(string invoiceId, string merchantId)
        {
            return await _runner.RunAsync(() => _api.GetAsync(invoiceId, merchantId));
        }

        public async Task<IEnumerable<InvoiceModel>> GetByMerchantIdAsync(string merchantId)
        {
            return await _runner.RunAsync(() => _api.GetByMerchantIdAsync(merchantId));
        }

        public async Task<InvoiceModel> CreateDraftAsync(NewInvoiceModel model)
        {
            return await _runner.RunAsync(() => _api.CreateDraftAsync(model));
        }

        public async Task UpdateDraftAsync(InvoiceModel model)
        {
            await _runner.RunAsync(() => _api.UpdateDraftAsync(model));
        }

        public async Task<InvoiceModel> GenerateAsync(NewInvoiceModel model)
        {
            return await _runner.RunAsync(() => _api.GenerateAsync(model));
        }

        public async Task<InvoiceModel> GenerateFromDraftAsync(InvoiceModel model)
        {
            return await _runner.RunAsync(() => _api.GenerateFromDraftAsync(model));
        }

        public async Task DeleteAsync(string invoiceId, string merchantId)
        {
            await _runner.RunAsync(() => _api.DeleteAsync(invoiceId, merchantId));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
