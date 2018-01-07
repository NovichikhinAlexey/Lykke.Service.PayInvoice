using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Client.Api;
using Lykke.Pay.Service.Invoces.Client.Models.File;
using Lykke.Pay.Service.Invoces.Client.Models.Invoice;
using Microsoft.Extensions.PlatformAbstractions;
using Refit;

namespace Lykke.Pay.Service.Invoces.Client
{
    public class PayInvoicesServiceClient : IDisposable, IPayInvoicesServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IInvoiceApi _invoiceApi;
        private readonly IFileApi _fileApi;
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

            _invoiceApi = RestService.For<IInvoiceApi>(_httpClient);
            _fileApi = RestService.For<IFileApi>(_httpClient);
            _runner = new ApiRunner();
        }

        public async Task<InvoiceModel> GetInvoiceAsync(string invoiceId, string merchantId)
        {
            return await _runner.RunAsync(() => _invoiceApi.GetAsync(invoiceId, merchantId));
        }

        public async Task<IEnumerable<InvoiceModel>> GetInvoicesByMerchantIdAsync(string merchantId)
        {
            return await _runner.RunAsync(() => _invoiceApi.GetByMerchantIdAsync(merchantId));
        }

        public async Task<InvoiceModel> CreateDraftInvoiceAsync(NewInvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoiceApi.CreateDraftAsync(model));
        }

        public async Task UpdateDraftInvoiceAsync(InvoiceModel model)
        {
            await _runner.RunAsync(() => _invoiceApi.UpdateDraftAsync(model));
        }

        public async Task<InvoiceModel> GenerateInvoiceAsync(NewInvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoiceApi.GenerateAsync(model));
        }

        public async Task<InvoiceModel> GenerateInvoiceFromDraftAsync(InvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoiceApi.GenerateFromDraftAsync(model));
        }

        public async Task DeleteInvoiceAsync(string invoiceId, string merchantId)
        {
            await _runner.RunAsync(() => _invoiceApi.DeleteAsync(invoiceId, merchantId));
        }

        public async Task<FileInfoModel> GetFileInfoAsync(string invoiceId, string fileId)
        {
            return await _runner.RunAsync(() => _fileApi.GetInfoAsync(invoiceId, fileId));
        }

        public async Task<IEnumerable<FileInfoModel>> GetFileInfoByInvoiceAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _fileApi.GetInfoByInvoiceAsync(invoiceId));
        }
        
        public async Task<byte[]> GetFileContentAsync(string invoiceId, string fileId)
        {
            HttpResponseMessage response = await _runner.RunAsync(() => _fileApi.GetFileAsync(invoiceId, fileId));

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task UploadFileAsync(string invoiceId, byte[] content, string fileName, string contentType)
        {
            var streamPart = new StreamPart(new MemoryStream(content), fileName, contentType);

            await _runner.RunAsync(() => _fileApi.UploadFileAsync(invoiceId, streamPart));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
