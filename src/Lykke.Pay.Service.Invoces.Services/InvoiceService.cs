using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Pay.Common;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Core.Services;
using Lykke.Pay.Service.Invoces.Services.ext;

namespace Lykke.Pay.Service.Invoces.Services
{
    [UsedImplicitly]
    public class InvoiceService : IInvoiceService<IInvoiceEntity>
    { 
        private readonly IInvoiceRepository _repository;
        private readonly ILog _log;

        public InvoiceService(IInvoiceRepository repository, ILog log)
        {
            _repository = repository;
            _log = log;
        }

        public async Task<bool> SaveInvoice(IInvoiceEntity invoice)
        {
            var  result = await _repository.SaveInvoice(invoice);
            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(SaveInvoice), 
                invoice.ToContext().AddParam("result", result).ToJson(), 
                $"Save invoce {invoice.InvoiceId}, for merchant {invoice.MerchantId} with result {result}");
            return result;
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
                    await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteInvoice), invoice.ToContext().ToJson(), $"Delete invoce {invoice.InvoiceId}, for merchant {invoice.MerchantId} with status {invoice.Status}");
                }
                else if (invoiceStatus == InvoiceStatus.Unpaid)
                {
                    invoice.Status = InvoiceStatus.Removed.ToString();
                    await _repository.SaveInvoice(invoice);
                    await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteInvoice),
                        invoice.ToContext().ToJson(),
                        $"Delete invoce {invoice.InvoiceId}, for merchant {invoice.MerchantId} with status {InvoiceStatus.Unpaid}");
                }
                else
                {
                    await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteInvoice), invoice.ToContext().ToJson(), $"CANNOT delete invoce {invoice.InvoiceId}, for merchant {invoice.MerchantId} in current status");
                }
            }
            
        }

        public async Task UploadFile(IFileEntity entity)
        {
            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(UploadFile), entity.ToContext().ToJson(), $"Upload file {entity.FileName} for invoce {entity.InvoiceId}");
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
            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(UploadFile), ContextEntityExt.EmptyContext().AddParam("InvoiceId", invoiceId).ToJson(), $"Delete file {invoiceId}");
            await _repository.DeleteFiles(invoiceId);
        }
    }
}