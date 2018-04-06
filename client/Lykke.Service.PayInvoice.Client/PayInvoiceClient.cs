using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Api;
using Lykke.Service.PayInvoice.Client.Models.Balances;
using Lykke.Service.PayInvoice.Client.Models.Employee;
using Lykke.Service.PayInvoice.Client.Models.File;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Refit;

namespace Lykke.Service.PayInvoice.Client
{
    public class PayInvoiceClient : IPayInvoiceClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IInvoicesApi _invoicesApi;
        private readonly IFilesApi _filesApi;
        private readonly IEmployeesApi _employeesApi;
        private readonly IBalancesApi _balancesApi;
        private readonly ApiRunner _runner;

        public PayInvoiceClient(PayInvoiceServiceClientSettings settings)
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

            _invoicesApi = RestService.For<IInvoicesApi>(_httpClient);
            _filesApi = RestService.For<IFilesApi>(_httpClient);
            _employeesApi = RestService.For<IEmployeesApi>(_httpClient);
            _balancesApi = RestService.For<IBalancesApi>(_httpClient);
            _runner = new ApiRunner();
        }

        public async Task<InvoiceModel> GetInvoiceAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetAsync(invoiceId));
        }

        public async Task<InvoiceDetailsModel> CheckoutInvoiceAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.CheckoutAsync(invoiceId));
        }

        public async Task<IEnumerable<InvoiceModel>> GetInvoicesAsync(string merchantId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetAllAsync(merchantId));
        }

        public async Task<InvoiceModel> GetInvoiceAsync(string merchantId, string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetByIdAsync(merchantId, invoiceId));
        }

        public async Task<InvoiceModel> CreateDraftInvoiceAsync(string merchantId, CreateDraftInvoiceModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"/api/merchants/{merchantId}/invoices/drafts", content);

            if (result.IsSuccessStatusCode)
            {
                string value = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<InvoiceModel>(value);
            }

            throw new ErrorResponseException("An error occurred  during calling api");

            //return await _runner.RunAsync(() => _invoicesApi.CreateDraftAsync(merchantId, model));
        }

        public async Task UpdateDraftInvoiceAsync(string merchantId, string invoiceId, CreateDraftInvoiceModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var result = await _httpClient.PutAsync($"/api/merchants/{merchantId}/invoices/{invoiceId}", content);

            if (!result.IsSuccessStatusCode)
                throw new ErrorResponseException("An error occurred  during calling api");
            
            //await _runner.RunAsync(() => _invoicesApi.UpdateDraftAsync(merchantId, invoiceId, model));
        }

        public async Task<InvoiceModel> CreateInvoiceAsync(string merchantId, CreateInvoiceModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"/api/merchants/{merchantId}/invoices", content);

            if (result.IsSuccessStatusCode)
            {
                string value = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<InvoiceModel>(value);
            }

            throw new ErrorResponseException("An error occurred  during calling api");
            //return await _runner.RunAsync(() => _invoicesApi.CreateAsync(merchantId, model));
        }

        public async Task<InvoiceModel> CreateInvoiceFromDraftAsync(string merchantId, string invoiceId, CreateInvoiceModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"/api/merchants/{merchantId}/invoices/{invoiceId}", content);

            if (result.IsSuccessStatusCode)
            {
                string value = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<InvoiceModel>(value);
            }

            throw new ErrorResponseException("An error occurred  during calling api");
            //return await _runner.RunAsync(() => _invoicesApi.CreateFromDraftAsync(merchantId, invoiceId,model));
        }

        public async Task DeleteInvoiceAsync(string merchantId, string invoiceId)
        {
            await _runner.RunAsync(() => _invoicesApi.DeleteAsync(merchantId, invoiceId));
        }

        public async Task<HistoryItemModel> GetInvoiceHistoryAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetHistoryAsync(invoiceId));
        }

        public async Task<IEnumerable<FileInfoModel>> GetFilesAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _filesApi.GetAllAsync(invoiceId));
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

        public async Task DeleteFileAsync(string invoiceId, string fileId)
        {
            await _runner.RunAsync(() => _filesApi.DeleteAsync(invoiceId, fileId));
        }

        public async Task<IReadOnlyList<EmployeeModel>> GetEmployeesAsync(string merchantId)
        {
            return await _runner.RunAsync(() => _employeesApi.GetAllAsync(merchantId));
        }

        public async Task<EmployeeModel> GetEmployeeAsync(string merchantId, string employeeId)
        {
            return await _runner.RunAsync(() => _employeesApi.GetByIdAsync(merchantId, employeeId));
        }

        public async Task<EmployeeModel> AddEmployeeAsync(string merchantId, CreateEmployeeModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"/api/merchants/{merchantId}/employees", content);

            if (result.IsSuccessStatusCode)
            {
                string value = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<EmployeeModel>(value);
            }

            throw new ErrorResponseException("An error occurred  during calling api");
            //return await _runner.RunAsync(() => _employeesApi.AddAsync(merchantId, model));
        }

        public async Task UpdateEmployeeAsync(string merchantId, string employeeId, CreateEmployeeModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var result = await _httpClient.PutAsync($"/api/merchants/{merchantId}/employees/{employeeId}", content);

            if (!result.IsSuccessStatusCode)
                throw new ErrorResponseException("An error occurred  during calling api");
            //await _runner.RunAsync(() => _employeesApi.UpdateAsync(merchantId, employeeId, model));
        }

        public async Task DeleteEmployeeAsync(string merchantId, string employeeId)
        {
            await _runner.RunAsync(() => _employeesApi.DeleteAsync(merchantId, employeeId));
        }

        public async Task<BalanceModel> GetBalanceAsync(string merchantId, string assetId)
        {
            return await _runner.RunAsync(() => _balancesApi.GetAsync(merchantId, assetId));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
