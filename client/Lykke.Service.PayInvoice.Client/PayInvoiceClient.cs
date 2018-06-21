using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Api;
using Lykke.Service.PayInvoice.Client.Models.Employee;
using Lykke.Service.PayInvoice.Client.Models.File;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Lykke.Service.PayInvoice.Client.Models.MerchantSetting;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Refit;

namespace Lykke.Service.PayInvoice.Client
{
    public class PayInvoiceClient : IPayInvoiceClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IInvoicesApi _invoicesApi;
        private readonly IMerchantsApi _merchantsApi;
        private readonly IDraftsApi _draftsApi;
        private readonly IFilesApi _filesApi;
        private readonly IEmployeesApi _employeesApi;
        private readonly IMerchantSettingsApi _merchantSettingsApi;
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
            _merchantsApi = RestService.For<IMerchantsApi>(_httpClient);
            _draftsApi = RestService.For<IDraftsApi>(_httpClient);
            _filesApi = RestService.For<IFilesApi>(_httpClient);
            _employeesApi = RestService.For<IEmployeesApi>(_httpClient);
            _merchantSettingsApi = RestService.For<IMerchantSettingsApi>(_httpClient);
            _runner = new ApiRunner();
        }

        public async Task<InvoiceModel> GetInvoiceAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetAsync(invoiceId));
        }

        public async Task<IReadOnlyList<InvoiceModel>> GetByFilter(IEnumerable<string> merchantIds, IEnumerable<string> clientMerchantIds, IEnumerable<string> statuses, bool? dispute, IEnumerable<string> billingCategories, decimal? greaterThan, decimal? lessThan)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetByFilter(merchantIds, clientMerchantIds, statuses, dispute, billingCategories, greaterThan, lessThan));
        }

        public async Task<InvoiceModel> CreateInvoiceAsync(CreateInvoiceModel model)
        {
            return await _runner.RunAsync(() => _invoicesApi.CreateAsync(model));
        }

        public async Task<InvoiceModel> CreateInvoiceAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.CreateFromDraftAsync(invoiceId));
        }

        public async Task<InvoiceModel> ChangePaymentAssetAsync(string invoiceId, string paymentAssetId)
        {
            return await _runner.RunAsync(() => _invoicesApi.ChangePaymentAssetAsync(invoiceId, paymentAssetId));
        }

        public async Task DeleteInvoiceAsync(string invoiceId)
        {
            await _runner.RunAsync(() => _invoicesApi.DeleteAsync(invoiceId));
        }

        public Task PayInvoicesAsync(PayInvoicesRequest model)
        {
            return _runner.RunAsync(() => _invoicesApi.PayInvoicesAsync(model));
        }

        public Task<decimal> GetSumToPayInvoicesAsync(GetSumToPayInvoicesRequest model)
        {
            return _runner.RunAsync(() => _invoicesApi.GetSumToPayInvoicesAsync(model));
        }

        public async Task<IReadOnlyList<HistoryItemModel>> GetInvoiceHistoryAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetHistoryAsync(invoiceId));
        }

        public async Task<IReadOnlyList<PaymentRequestHistoryItem>> GetPaymentRequestsAsync(string invoiceId)
        {
            return await _runner.RunAsync(() => _invoicesApi.GetPaymentRequestsAsync(invoiceId));
        }

        public async Task<IEnumerable<InvoiceModel>> GetMerchantInvoicesAsync(string merchantId)
        {
            return await _runner.RunAsync(() => _merchantsApi.GetAllAsync(merchantId));
        }

        public async Task<MerchantSetting> GetMerchantSettingAsync(string merchantId)
        {
            return await _runner.RunAsync(() => _merchantSettingsApi.GetByIdAsync(merchantId));
        }

        public async Task<MerchantSetting> SetMerchantSettingAsync(MerchantSetting model)
        {
            return await _runner.RunAsync(() => _merchantSettingsApi.SetAsync(model));
        }

        public Task<string> GetBaseAssetAsync(string merchantId)
        {
            return _runner.RunAsync(() => _merchantSettingsApi.GetBaseAssetByIdAsync(merchantId));
        }

        public Task SetBaseAssetAsync(UpdateBaseAssetRequest model)
        {
            return _runner.RunAsync(() => _merchantSettingsApi.SetBaseAssetAsync(model));
        }

        public async Task<InvoiceModel> CreateDraftInvoiceAsync(CreateInvoiceModel model)
        {
            return await _runner.RunAsync(() => _draftsApi.CreateAsync(model));
        }

        public async Task UpdateDraftInvoiceAsync(UpdateInvoiceModel model)
        {
            await _runner.RunAsync(() => _draftsApi.UpdateAsync(model));
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

        public async Task<IReadOnlyList<EmployeeModel>> GetEmployeesAsync()
        {
            return await _runner.RunAsync(() => _employeesApi.GetAllAsync());
        }

        public async Task<EmployeeModel> GetEmployeeAsync(string employeeId)
        {
            return await _runner.RunAsync(() => _employeesApi.GetByIdAsync(employeeId));
        }

        public async Task<EmployeeModel> AddEmployeeAsync(CreateEmployeeModel model)
        {
            //var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            //var result = await _httpClient.PostAsync($"/api/merchants/{merchantId}/employees", content);

            //if (result.IsSuccessStatusCode)
            //{
            //    string value = await result.Content.ReadAsStringAsync();
            //    return JsonConvert.DeserializeObject<EmployeeModel>(value);
            //}

            //throw new ErrorResponseException("An error occurred  during calling api");
            return await _runner.RunAsync(() => _employeesApi.AddAsync(model));
        }

        public async Task UpdateEmployeeAsync(UpdateEmployeeModel model)
        {
            //var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            //var result = await _httpClient.PutAsync($"/api/merchants/{merchantId}/employees/{employeeId}", content);

            //if (!result.IsSuccessStatusCode)
            //    throw new ErrorResponseException("An error occurred  during calling api");
            await _runner.RunAsync(() => _employeesApi.UpdateAsync(model));
        }

        public async Task DeleteEmployeeAsync(string employeeId)
        {
            await _runner.RunAsync(() => _employeesApi.DeleteAsync(employeeId));
        }

        public Task<EmployeeModel> GetEmployeeByEmailAsync(string email)
        {
            return _runner.RunAsync(() => _employeesApi.GetByEmailAsync(email));
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<IReadOnlyList<EmployeeModel>> GetEmployeesAsync(string merchantId)
        {
            var result = await _httpClient.GetAsync($"/api/merchants/{merchantId}/employees");

            if (result.IsSuccessStatusCode)
            {
                string value = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IReadOnlyList<EmployeeModel>>(value);
            }

            throw new ErrorResponseException("An error occurred  during calling api");
        }

        public Task MarkDisputeAsync(MarkInvoiceDisputeRequest model)
        {
            return _runner.RunAsync(() => _invoicesApi.MarkDisputeAsync(model));
        }

        public Task CancelDisputeAsync(CancelInvoiceDisputeRequest model)
        {
            return _runner.RunAsync(() => _invoicesApi.CancelDisputeAsync(model));
        }

        public Task<InvoiceDisputeInfoResponse> GetInvoiceDisputeInfoAsync(string invoiceId)
        {
            return _runner.RunAsync(() => _invoicesApi.GetInvoiceDisputeInfoAsync(invoiceId));
        }
    }
}
