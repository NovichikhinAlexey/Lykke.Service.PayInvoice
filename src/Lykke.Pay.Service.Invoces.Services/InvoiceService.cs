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

        public async Task<List<IInvoiceEntity>> GetInvoices()
        {
            return await _repository.GetInvoices();
        }

        public async Task<IInvoiceEntity> GetInvoice(string invoiceId)
        {
            return await _repository.GetInvoice(invoiceId);
        }

        public async Task DeleteInvoice(string invoiceId)
        {
            var invoice  = await _repository.GetInvoice(invoiceId);
            if (invoice != null)
            {
                var invoiceStatus = invoice.Status.ParsePayEnum<InvoiceStatus>();
                if (invoiceStatus == InvoiceStatus.Draft)
                {
                    await _repository.DeleteInvoice(invoiceId);
                }
                else if (invoiceStatus == InvoiceStatus.Unpaid)
                {
                    invoice.Status = InvoiceStatus.Removed.ToString();
                    await _repository.SaveInvoice(invoice);
                }
            }
            
        }
    }
}