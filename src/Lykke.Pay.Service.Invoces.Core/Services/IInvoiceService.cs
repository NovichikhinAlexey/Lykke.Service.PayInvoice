using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Core.Services
{
    public interface IInvoiceService<TInvoiceEntity>
        where TInvoiceEntity : IInvoiceEntity
    {
        Task<bool> SaveInvoice(TInvoiceEntity invoice);

        Task<List<TInvoiceEntity>> GetInvoices();

        Task<TInvoiceEntity> GetInvoice(string invoiceId);
        Task DeleteInvoice(string invoiceId);
    }
}