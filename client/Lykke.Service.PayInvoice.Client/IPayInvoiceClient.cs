using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Employee;
using Lykke.Service.PayInvoice.Client.Models.File;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Client
{
    public interface IPayInvoiceClient
    {
        /// <summary>
        /// Returns invoice by id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The invoice.</returns>
        Task<InvoiceModel> GetInvoiceAsync(string invoiceId);

        /// <summary>
        /// Returns invoices by filter
        /// </summary>
        /// <param name="merchantId">The merchant id</param>
        /// <param name="clientMerchantId">The merchant id of the client</param>
        /// <param name="statuses">The statuses</param>
        /// <param name="dispute">The dispute attribute</param>
        /// <param name="billingCategories">The billing categories</param>
        /// <param name="greaterThan">The greater than number for filtering</param>
        /// <param name="lessThan">The less than number for filtering</param>
        /// <returns>A collection of invoices</returns>
        Task<IReadOnlyList<InvoiceModel>> GetByFilter(string merchantId, string clientMerchantId, string statuses, bool? dispute, string billingCategories, int? greaterThan, int? lessThan);

        /// <summary>
        /// Returns invoice history.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>A collection of invoice history items.</returns>
        Task<IReadOnlyList<HistoryItemModel>> GetInvoiceHistoryAsync(string invoiceId);

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
        /// Deletes an invoice by specified id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        Task DeleteInvoiceAsync(string invoiceId);

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
        /// Updates an employee.
        /// </summary>
        /// <param name="model">The employee info.</param>
        Task UpdateEmployeeAsync(UpdateEmployeeModel model);
        
        /// <summary>
        /// Deletes an employee.
        /// </summary>
        /// <param name="employeeId">The employee id.</param>
        Task DeleteEmployeeAsync(string employeeId);
    }
}