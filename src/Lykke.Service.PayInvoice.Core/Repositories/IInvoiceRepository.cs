using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<IInvoice>> GetAsync();
        Task<IInvoice> GetAsync(string invoiceId);
        Task<IInvoice> GetAsync(string merchantId, string invoiceId);
        Task<IEnumerable<IInvoice>> GetByMerchantIdAsync(string merchantId);
        Task<IInvoice> GetByAddressAsync(string address);
        Task InsertAsync(IInvoice invoice);
        Task UpdateAsync(IInvoice invoice);
        Task DeleteAsync(string merchantId, string invoiceId);
    }
}