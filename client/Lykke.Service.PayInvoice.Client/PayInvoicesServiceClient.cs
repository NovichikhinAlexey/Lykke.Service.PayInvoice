using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Api;
using Lykke.Service.PayInvoice.Client.Models.Employee;
using Lykke.Service.PayInvoice.Client.Models.File;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Microsoft.Extensions.PlatformAbstractions;
using Refit;

namespace Lykke.Service.PayInvoice.Client
{
    public class PayInvoicesServiceClient : IPayInvoicesServiceClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IPayInvoicesApi _invoicesApi;
        private readonly IFilesApi _filesApi;
        private readonly IEmployeesApi _employeesApi;
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

            _invoicesApi = RestService.For<IPayInvoicesApi>(_httpClient);
            _filesApi = RestService.For<IFilesApi>(_httpClient);
            _employeesApi = RestService.For<IEmployeesApi>(_httpClient);
            _runner = new ApiRunner();
        }

        public async Task<InvoiceDetailsModel> GetInvoiceDetailsAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetDetailsAsync(invoiceId));
        }

        public async Task<IEnumerable<InvoiceModel>> GetInvoicesAsync(string merchantId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetAsync(merchantId));
        }
        
        public async Task<InvoiceModel> GetInvoiceAsync(string merchantId, string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetAsync(merchantId, invoiceId));
        }

        public async Task<InvoiceModel> CreateDraftInvoiceAsync(string merchantId, CreateDraftInvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoicesApi.CreateDraftAsync(merchantId, model));
        }

        public async Task UpdateDraftInvoiceAsync(string merchantId, string invoiceId, CreateDraftInvoiceModel model)
        {
            await _runner.RunAsync(() => _invoicesApi.UpdateDraftAsync(merchantId, invoiceId, model));
        }

        public async Task<InvoiceModel> CreateInvoiceAsync(string merchantId, CreateInvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoicesApi.CreateAsync(merchantId, model));
        }

        public async Task<InvoiceModel> CreateInvoiceFromDraftAsync(string merchantId, string invoiceId, CreateInvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoicesApi.CreateFromDraftAsync(merchantId, invoiceId,model));
        }

        public async Task DeleteInvoiceAsync(string merchantId, string invoiceId)
        {
            await _runner.RunAsync(() => _invoicesApi.DeleteAsync(merchantId, invoiceId));
        }

        public async Task<IEnumerable<FileInfoModel>> GetFilesAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _filesApi.GetAsync(invoiceId));
        }
        
        public async Task<byte[]> GetFileAsync(string invoiceId, string fileId)
        {
            HttpResponseMessage response = await _runner.RunAsync(() => _filesApi.GetAsync(invoiceId, fileId));

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task UploadFileAsync(string invoiceId, byte[] content, string fileName, string contentType)
        {
            var streamPart = new StreamPart(new MemoryStream(content), fileName, contentType);

            await _runner.RunAsync(() => _filesApi.UploadAsync(invoiceId, streamPart));
        }

        public async Task<IReadOnlyList<EmployeeModel>> GetEmployeesAsync(string merchantId)
        {
            return await _runner.RunAsync(() => _employeesApi.GetAsync(merchantId));
        }
        
        public async Task<EmployeeModel> GetEmployeeAsync(string merchantId, string employeeId)
        {
            return await _runner.RunAsync(() => _employeesApi.GetAsync(merchantId, employeeId));
        }
        
        public async Task<EmployeeModel> AddEmployeeAsync(string merchantId, CreateEmployeeModel model)
        {
            return await _runner.RunAsync(() => _employeesApi.AddAsync(merchantId, model));
        }
        
        public async Task UpdateEmployeeAsync(string merchantId, string employeeId, CreateEmployeeModel model)
        {
            await _runner.RunAsync(() => _employeesApi.UpdateAsync(merchantId, employeeId, model));
        }
        
        public async Task DeleteEmployeeAsync(string merchantId, string employeeId)
        {
            await _runner.RunAsync(() => _employeesApi.DeleteAsync(merchantId, employeeId));
        }
        
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
