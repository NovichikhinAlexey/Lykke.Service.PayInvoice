using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Api;
using Lykke.Service.PayInvoice.Client.Models.File;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Microsoft.Extensions.PlatformAbstractions;
using Refit;

namespace Lykke.Service.PayInvoice.Client
{
    public class PayInvoicesServiceClient : IPayInvoicesServiceClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IPayInvoiceApi _invoiceApi;
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

            _invoiceApi = RestService.For<IPayInvoiceApi>(_httpClient);
            _fileApi = RestService.For<IFileApi>(_httpClient);
            _runner = new ApiRunner();
        }

        public async Task<InvoiceDetailsModel> GetInvoiceDetailsAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _invoiceApi.GetDetailsAsync(invoiceId));
        }

        public async Task<IEnumerable<InvoiceModel>> GetInvoicesAsync(string merchantId)
        {
            return await _runner.RunAsync(() => _invoiceApi.GetAsync(merchantId));
        }
        
        public async Task<InvoiceModel> GetInvoiceAsync(string merchantId, string invoiceId)
        {
            return await _runner.RunAsync(() => _invoiceApi.GetAsync(merchantId, invoiceId));
        }

        public async Task<InvoiceModel> CreateDraftInvoiceAsync(string merchantId, CreateDraftInvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoiceApi.CreateDraftAsync(merchantId, model));
        }

        public async Task UpdateDraftInvoiceAsync(string merchantId, string invoiceId, CreateDraftInvoiceModel model)
        {
            await _runner.RunAsync(() => _invoiceApi.UpdateDraftAsync(merchantId, invoiceId, model));
        }

        public async Task<InvoiceModel> CreateInvoiceAsync(string merchantId, CreateInvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoiceApi.CreateAsync(merchantId, model));
        }

        public async Task<InvoiceModel> CreateInvoiceFromDraftAsync(string merchantId, string invoiceId, CreateInvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoiceApi.CreateFromDraftAsync(merchantId, invoiceId,model));
        }

        public async Task DeleteInvoiceAsync(string merchantId, string invoiceId)
        {
            await _runner.RunAsync(() => _invoiceApi.DeleteAsync(merchantId, invoiceId));
        }

        public async Task<IEnumerable<FileInfoModel>> GetFilesAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _fileApi.GetAsync(invoiceId));
        }
        
        public async Task<byte[]> GetFileAsync(string invoiceId, string fileId)
        {
            HttpResponseMessage response = await _runner.RunAsync(() => _fileApi.GetAsync(invoiceId, fileId));

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task UploadFileAsync(string invoiceId, byte[] content, string fileName, string contentType)
        {
            var streamPart = new StreamPart(new MemoryStream(content), fileName, contentType);

            await _runner.RunAsync(() => _fileApi.UploadAsync(invoiceId, streamPart));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
