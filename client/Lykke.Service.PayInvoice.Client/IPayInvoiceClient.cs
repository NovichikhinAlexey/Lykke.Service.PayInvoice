using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Balances;
using Lykke.Service.PayInvoice.Client.Models.Employee;
using Lykke.Service.PayInvoice.Client.Models.File;
using Lykke.Service.PayInvoice.Client.Models.Invoice;

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
        /// Returns checkout invoice details.
        /// </summary>
        /// <returns>The invoice details.</returns>
        Task<InvoiceDetailsModel> CheckoutInvoiceAsync(string invoiceId);
        
        /// <summary>
        /// Returns an invoice.
        /// </summary>
        /// <returns>The invoice.</returns>
        Task<InvoiceModel> GetInvoiceAsync(string merchantId, string invoiceId);
        
        /// <summary>
        /// Returns invoices by merchant id.
        /// </summary>
        /// <returns>The collection of invoices.</returns>
        Task<IEnumerable<InvoiceModel>> GetInvoicesAsync(string merchantId);
        
        /// <summary>
        /// Creates draft invoice.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="model">The model that describe an invoice.</param>
        Task<InvoiceModel> CreateDraftInvoiceAsync(string merchantId, CreateDraftInvoiceModel model);
        
        /// <summary>
        /// Updates draft invoice.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="model">The model that describe an invoice.</param>
        Task UpdateDraftInvoiceAsync(string merchantId, string invoiceId, CreateDraftInvoiceModel model);
        
        /// <summary>
        /// Creates an invoice.
        /// </summary>
        /// <param name="merchantId">The invoice id.</param>
        /// <param name="model">The model that describe an invoice.</param>
        Task<InvoiceModel> CreateInvoiceAsync(string merchantId, CreateInvoiceModel model);
        
        /// <summary>
        /// Updates an invoice.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="model">The model that describe an invoice.</param>
        Task<InvoiceModel> CreateInvoiceFromDraftAsync(string merchantId, string invoiceId, CreateInvoiceModel model);
        
        /// <summary>
        /// Deletes an invoice by specified id.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        Task DeleteInvoiceAsync(string merchantId, string invoiceId);
        
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
        
        /// <summary>
        /// Returns an employee.
        /// </summary>
        /// <returns>The employee.</returns>
        Task<EmployeeModel> GetEmployeeAsync(string merchantId, string employeeId);
        
        /// <summary>
        /// Creates an employee.
        /// </summary>
        /// <param name="merchantId">The merchan id.</param>
        /// <param name="model">The employee info.</param>
        Task<EmployeeModel> AddEmployeeAsync(string merchantId, CreateEmployeeModel model);
        
        /// <summary>
        /// Updates an employee.
        /// </summary>
        /// <param name="merchantId">The merchan id.</param>
        /// <param name="employeeId">The employee id.</param>
        /// <param name="model">The employee info.</param>
        Task UpdateEmployeeAsync(string merchantId, string employeeId, CreateEmployeeModel model);
        
        /// <summary>
        /// Deletes an employee.
        /// </summary>
        /// <param name="merchantId">The merchan id.</param>
        /// <param name="employeeId">The employee id.</param>
        Task DeleteEmployeeAsync(string merchantId, string employeeId);

        /// <summary>
        /// Returns merchant asset balance.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="assetId">The asset id.</param>
        /// <returns>The merchant asset balance.</returns>
        Task<BalanceModel> GetBalanceAsync(string merchantId, string assetId);
    }
}