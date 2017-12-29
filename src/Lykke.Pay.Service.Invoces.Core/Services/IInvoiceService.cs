using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Core.Services
{
    public interface IInvoiceService<TInvoiceEntity>
        where TInvoiceEntity : IInvoiceEntity
    {
        Task<bool> SaveInvoice(TInvoiceEntity invoice);
        Task<List<TInvoiceEntity>> GetInvoices(string merchantId);
        Task<TInvoiceEntity> GetInvoice(string merchantId, string invoiceId);
        Task<IInvoiceEntity> GetInvoiceByAddress(string address);
        Task DeleteInvoice(string merchantId, string invoiceId);
        Task UploadFile(IFileEntity entity);
        Task<List<IFileMetaEntity>> GetFileMetaList(string invoiceId);
        Task<IFileEntity> GetFileEntity(string invoiceId, string fileId);
        Task DeleteFiles(string invoiceId);
    }
}