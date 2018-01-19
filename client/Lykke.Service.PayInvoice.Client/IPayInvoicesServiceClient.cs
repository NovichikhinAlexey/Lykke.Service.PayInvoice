using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.File;
using Lykke.Service.PayInvoice.Client.Models.Invoice;

namespace Lykke.Service.PayInvoice.Client
{
    public interface IPayInvoicesServiceClient
    {
        Task<InvoiceSummaryModel> GetInvoiceSummaryAsync(string invoiceId);
        Task<InvoiceModel> GetInvoiceAsync(string merchantId, string invoiceId);
        Task<IEnumerable<InvoiceModel>> GetInvoicesAsync(string merchantId);
        Task<InvoiceModel> CreateDraftInvoiceAsync(NewDraftInvoiceModel model);
        Task UpdateDraftInvoiceAsync(UpdateDraftInvoiceModel model);
        Task<InvoiceModel> GenerateInvoiceAsync(NewInvoiceModel model);
        Task<InvoiceModel> GenerateInvoiceFromDraftAsync(UpdateInvoiceModel model);
        Task DeleteInvoiceAsync(string merchantId, string invoiceId);
        Task<FileInfoModel> GetFileInfoAsync(string invoiceId, string fileId);
        Task<IEnumerable<FileInfoModel>> GetFileInfoByInvoiceAsync(string invoiceId);
        Task<byte[]> GetFileContentAsync(string invoiceId, string fileId);
        Task UploadFileAsync(string invoiceId, byte[] content, string fileName, string contentType);
    }
}