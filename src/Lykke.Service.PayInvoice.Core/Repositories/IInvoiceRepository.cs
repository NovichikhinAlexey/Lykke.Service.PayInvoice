using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IInvoiceRepository
    {
        Task<IReadOnlyList<IInvoice>> GetAsync();
        Task<IReadOnlyList<IInvoice>> GetAsync(string merchantId);
        Task<IInvoice> GetAsync(string merchantId, string invoiceId);
        Task<IInvoice> FindAsync(string invoiceId);
        Task<IInvoice> InsertAsync(IInvoice invoice);
        Task ReplaceAsync(IInvoice invoice);
        Task SetStatusAsync(string merchantId, string invoiceId, InvoiceStatus status);
        Task DeleteAsync(string merchantId, string invoiceId);
    }
}