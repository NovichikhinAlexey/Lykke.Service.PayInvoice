using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Pay.Service.Invoces.Core.Domain
{
    public interface IInvoiceRepository
    {
        Task<bool> SaveInvoice(IInvoiceEntity invoice);

        Task<List<IInvoiceEntity>> GetInvoices();

        Task<IInvoiceEntity> GetInvoice(string invoiceId);
    }
}