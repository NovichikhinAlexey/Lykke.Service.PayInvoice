using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IInvoiceService
    {
        Task<IReadOnlyList<IInvoice>> GetAsync();
        Task<IReadOnlyList<IInvoice>> GetAsync(string merchantId);
        Task<IInvoice> GetAsync(string merchantId, string invoiceId);
        Task<IInvoice> CreateDraftAsync(IInvoice invoice);
        Task UpdateDraftAsync(IInvoice invoice);
        Task<IInvoice> CreateAsync(IInvoice invoice);
        Task<IInvoice> CreateFromDraftAsync(IInvoice invoice);
        Task SetStatus(string invoiceId, InvoiceStatus status);
        Task<IInvoiceDetails> GetDetailsAsync(string invoiceId);
        Task DeleteAsync(string merchantId, string invoiceId);
    }
}