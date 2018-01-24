using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.File;
using Lykke.Service.PayInvoice.Client.Models.Invoice;

namespace Lykke.Service.PayInvoice.Client
{
    public interface IPayInvoiceClient
    {
        Task<InvoiceDetailsModel> GetInvoiceDetailsAsync(string invoiceId);
        Task<InvoiceModel> GetInvoiceAsync(string merchantId, string invoiceId);
        Task<IEnumerable<InvoiceModel>> GetInvoicesAsync(string merchantId);
        Task<InvoiceModel> CreateDraftInvoiceAsync(string merchantId, CreateDraftInvoiceModel model);
        Task UpdateDraftInvoiceAsync(string merchantId, string invoiceId, CreateDraftInvoiceModel model);
        Task<InvoiceModel> CreateInvoiceAsync(string merchantId, CreateInvoiceModel model);
        Task<InvoiceModel> CreateInvoiceFromDraftAsync(string merchantId, string invoiceId, CreateInvoiceModel model);
        Task DeleteInvoiceAsync(string merchantId, string invoiceId);
        Task<IEnumerable<FileInfoModel>> GetFilesAsync(string invoiceId);
        Task<byte[]> GetFileAsync(string invoiceId, string fileId);
        Task UploadFileAsync(string invoiceId, byte[] content, string fileName, string contentType);
    }
}