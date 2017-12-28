using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Pay.Service.Invoces.Core.Domain
{
    public interface IInvoiceRepository
    {
        Task<bool> SaveInvoice(IInvoiceEntity invoice);
        Task<List<IInvoiceEntity>> GetInvoices(string merchantId);
        Task<IInvoiceEntity> GetInvoice(string merchantId, string invoiceId);
        Task DeleteInvoice(string merchantId, string invoiceId);
        Task UploadFile(IFileEntity entity);
        Task<List<IFileMetaEntity>> GetFileMeta(string invoiceId);
        Task<IFileEntity> GetFileEntity(string invoiceId, string fileId);
        Task DeleteFiles(string invoiceId);
    }
}