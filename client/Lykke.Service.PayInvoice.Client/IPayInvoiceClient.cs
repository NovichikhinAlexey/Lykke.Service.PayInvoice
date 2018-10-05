using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Employee;
using Lykke.Service.PayInvoice.Client.Models.File;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Lykke.Service.PayInvoice.Client.Models.MerchantSetting;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;

namespace Lykke.Service.PayInvoice.Client
{
    /// <summary>
    /// PayInvoice service client
    /// </summary>
    public interface IPayInvoiceClient
    {
        /// <summary>
        /// Returns invoice by id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The invoice.</returns>
        Task<InvoiceModel> GetInvoiceAsync(string invoiceId);

        /// <summary>
        /// Gets whether merchant has any invoice
        /// </summary>
        /// <param name="merchantId">The merchant id</param>
        Task<bool> HasAnyInvoiceAsync(string merchantId);

        /// <summary>
        /// Returns invoices by filter
        /// </summary>
        /// <param name="merchantIds">The merchant ids (e.g. ?merchantIds=one&amp;merchantIds=two)</param>
        /// <param name="clientMerchantIds">The merchant ids of the clients (e.g. ?clientMerchantIds=one&amp;clientMerchantIds=two)</param>
        /// <param name="statuses">The statuses (e.g. ?statuses=one&amp;statuses=two)</param>
        /// <param name="dispute">The dispute attribute</param>
        /// <param name="billingCategories">The billing categories (e.g. ?billingCategories=one&amp;billingCategories=two)</param>
        /// <param name="greaterThan">The greater than number for filtering</param>
        /// <param name="lessThan">The less than number for filtering</param>
        /// <returns>A collection of invoices</returns>
        Task<IReadOnlyList<InvoiceModel>> GetByFilter(IEnumerable<string> merchantIds, IEnumerable<string> clientMerchantIds, IEnumerable<string> statuses, bool? dispute, IEnumerable<string> billingCategories, decimal? greaterThan, decimal? lessThan);

        /// <summary>
        /// Returns invoices by payments filter
        /// </summary>
        /// <param name="merchantId">The merchant ids (e.g. ?merchantIds=one)</param>
        /// <param name="statuses">The statuses (e.g. ?statuses=one&amp;statuses=two)</param>
        /// <param name="dateFrom">The date from which to take</param>
        /// <param name="dateTo">The date until which to take</param>
        /// <param name="searchText">Search in text fields</param>
        /// <param name="take">The number of records to take</param>
        Task<GetByPaymentsFilterResponse> GetByPaymentsFilterAsync(
            string merchantId,
            IEnumerable<string> statuses,
            DateTime? dateFrom,
            DateTime? dateTo,
            string searchText,
            int? take
        );

        /// <summary>
        /// Returns invoice history.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>A collection of invoice history items.</returns>
        Task<IReadOnlyList<HistoryItemModel>> GetInvoiceHistoryAsync(string invoiceId);

        /// <summary>
        /// Returns invoice's payment requests
        /// </summary>
        /// <param name="invoiceId">The invoice id</param>
        /// <returns>A collection of invoice's payment requests</returns>
        Task<IReadOnlyList<PaymentRequestHistoryItem>> GetPaymentRequestsAsync(string invoiceId);

        /// <summary>
        /// Creates an invoice.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        /// <returns>Created invoice.</returns>
        Task<InvoiceModel> CreateInvoiceAsync(CreateInvoiceModel model);

        /// <summary>
        /// Updates an invoice from draft.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>Created invoice.</returns>
        Task<InvoiceModel> CreateInvoiceAsync(string invoiceId);

        /// <summary>
        /// Change payment asset of the invoice by creating new payment request with new asset
        /// </summary>
        /// <param name="invoiceId">The invoice id</param>
        /// <param name="paymentAssetId">The payment asset id</param>
        /// <returns></returns>
        Task<InvoiceModel> ChangePaymentAssetAsync(string invoiceId, string paymentAssetId);

        /// <summary>
        /// Deletes an invoice by specified id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        Task DeleteInvoiceAsync(string invoiceId);

        /// <summary>
        /// Pay invoices
        /// </summary>
        /// <param name="model">The model for paying invoices</param>
        Task PayInvoicesAsync(PayInvoicesRequest model);

        /// <summary>
        /// Get sum for paying invoices
        /// </summary>
        /// <param name="model">The model for getting sum</param>
        Task<decimal> GetSumToPayInvoicesAsync(GetSumToPayInvoicesRequest model);

        /// <summary>
        /// Mark invoice as Dispute
        /// </summary>
        /// <param name="model">The marking invoice dispute request</param>
        Task MarkDisputeAsync(MarkInvoiceDisputeRequest model);

        /// <summary>
        /// Cancel dispute
        /// </summary>
        /// <param name="model">Cancel dispute request</param>
        Task CancelDisputeAsync(CancelInvoiceDisputeRequest model);

        /// <summary>
        /// Get invoice's dispute information
        /// </summary>
        /// <param name="invoiceId">The invoice id</param>
        /// <returns></returns>
        Task<InvoiceDisputeInfoResponse> GetInvoiceDisputeInfoAsync(string invoiceId);

        /// <summary>
        /// Returns invoices by merchant id.
        /// </summary>
        /// <returns>The collection of invoices.</returns>
        Task<IEnumerable<InvoiceModel>> GetMerchantInvoicesAsync(string merchantId);

        /// <summary>
        /// Returns merchant setting by id
        /// </summary>
        Task<MerchantSetting> GetMerchantSettingAsync(string merchantId);

        /// <summary>
        /// Create or update merchant setting
        /// </summary>
        /// <param name="merchantSetting">The model to create or update merchant setting</param>
        Task<MerchantSetting> SetMerchantSettingAsync(MerchantSetting model);

        /// <summary>
        /// Get base asset
        /// </summary>
        Task<string> GetBaseAssetAsync(string merchantId);

        /// <summary>
        /// Update base asset
        /// </summary>
        Task SetBaseAssetAsync(UpdateBaseAssetRequest model);

        /// <summary>
        /// Creates draft invoice.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        /// <returns>Created draft invoice.</returns>
        Task<InvoiceModel> CreateDraftInvoiceAsync(CreateInvoiceModel model);

        /// <summary>
        /// Updates draft invoice.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        Task UpdateDraftInvoiceAsync(UpdateInvoiceModel model);

        /// <summary>
        /// Returns a collection of invoice files.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The collection of file info.</returns>
        Task<IEnumerable<FileInfoModel>> GetFilesAsync(string invoiceId);
        
        /// <summary>
        /// Returns file content.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="fileId">The file id.</param>
        Task<byte[]> GetFileAsync(string invoiceId, string fileId);

        /// <summary>
        /// Saves file.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="content">The file content.</param>
        /// <param name="fileName">The file name with extension.</param>
        /// <param name="contentType">The file mime type.</param>
        Task UploadFileAsync(string invoiceId, byte[] content, string fileName, string contentType);

        /// <summary>
        /// Deletes file.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="fileId">The file id.</param>
        Task DeleteFileAsync(string invoiceId, string fileId);

        /// <summary>
        /// Returns merchant employees.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <returns>The collection of employees.</returns>
        Task<IReadOnlyList<EmployeeModel>> GetEmployeesAsync(string merchantId);
        Task<IReadOnlyList<EmployeeModel>> GetEmployeesAsync();
        /// <summary>
        /// Returns an employee.
        /// </summary>
        /// <returns>The employee.</returns>
        Task<EmployeeModel> GetEmployeeAsync(string employeeId);
        
        /// <summary>
        /// Creates an employee.
        /// </summary>
        /// <param name="model">The employee info.</param>
        Task<EmployeeModel> AddEmployeeAsync(CreateEmployeeModel model);

        /// <summary>
        /// Mark employee deleted.
        /// </summary>
        /// <param name="model">Mark employee deleted request.</param>
        Task MarkEmployeeDeletedAsync(MarkEmployeeDeletedRequest model);
        
        /// <summary>
        /// Updates an employee.
        /// </summary>
        /// <param name="model">The employee info.</param>
        Task UpdateEmployeeAsync(UpdateEmployeeModel model);
        
        /// <summary>
        /// Deletes an employee.
        /// </summary>
        /// <param name="employeeId">The employee id.</param>
        Task DeleteEmployeeAsync(string employeeId);

        /// <summary>
        /// Returns an employee by email
        /// </summary>
        /// <param name="email">The employee email</param>
        /// <returns>The employee</returns>
        Task<EmployeeModel> GetEmployeeByEmailAsync(string email);
    }
}
