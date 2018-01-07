using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Client.Models.File;
using Lykke.Pay.Service.Invoces.Client.Models.Invoice;

namespace Lykke.Pay.Service.Invoces.Client
{
    public interface IPayInvoicesServiceClient
    {
        Task<InvoiceModel> GetInvoiceAsync(string invoiceId, string merchantId);
        Task<IEnumerable<InvoiceModel>> GetInvoicesByMerchantIdAsync(string merchantId);
        Task<InvoiceModel> CreateDraftInvoiceAsync(NewInvoiceModel model);
        Task UpdateDraftInvoiceAsync(InvoiceModel model);
        Task<InvoiceModel> GenerateInvoiceAsync(NewInvoiceModel model);
        Task<InvoiceModel> GenerateInvoiceFromDraftAsync(InvoiceModel model);
        Task DeleteInvoiceAsync(string invoiceId, string merchantId);
        Task<FileInfoModel> GetFileInfoAsync(string invoiceId, string fileId);
        Task<IEnumerable<FileInfoModel>> GetFileInfoByInvoiceAsync(string invoiceId);
        Task<byte[]> GetFileContentAsync(string invoiceId, string fileId);
        Task UploadFileAsync(string invoiceId, byte[] content, string fileName, string contentType);
    }
}