using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Pay.Common;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Core.Services;

namespace Lykke.Pay.Service.Invoces.Services
{
    [UsedImplicitly]
    public class InvoiceService : IInvoiceService<IInvoiceEntity>
    { 

        private readonly IInvoiceRepository _repository;
        public InvoiceService(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> SaveInvoice(IInvoiceEntity invoice)
        {
            return await _repository.SaveInvoice(invoice);
        }

        public async Task<List<IInvoiceEntity>> GetInvoices(string merchantId)
        {
            return await _repository.GetInvoices(merchantId);
        }

        public async Task<IInvoiceEntity> GetInvoice(string merchantId, string invoiceId)
        {
            return await _repository.GetInvoice(merchantId, invoiceId);
        }

        public async Task<IInvoiceEntity> GetInvoiceByAddress(string address)
        {
            return await _repository.GetInvoiceByAddress(address);
        }

        public async Task DeleteInvoice(string merchantId, string invoiceId)
        {
            var invoice  = await _repository.GetInvoice(merchantId, invoiceId);
            if (invoice != null)
            {
                var invoiceStatus = invoice.Status.ParsePayEnum<InvoiceStatus>();
                if (invoiceStatus == InvoiceStatus.Draft)
                {
                    await _repository.DeleteInvoice(merchantId, invoiceId);
                    await _repository.DeleteFiles(invoiceId);
                }
                else if (invoiceStatus == InvoiceStatus.Unpaid)
                {
                    invoice.Status = InvoiceStatus.Removed.ToString();
                    await _repository.SaveInvoice(invoice);
                }
            }
            
        }

        public async Task UploadFile(IFileEntity entity)
        {
            await _repository.UploadFile(entity);
        }

        public async Task<List<IFileMetaEntity>> GetFileMetaList(string invoiceId)
        {
            return await _repository.GetFileMeta(invoiceId);
        }

        public async Task<IFileEntity> GetFileEntity(string invoiceId, string fileId)
        {
            return await _repository.GetFileEntity(invoiceId, fileId);
        }


        public async Task DeleteFiles(string invoiceId)
        {
            await _repository.DeleteFiles(invoiceId);
        }
    }
}