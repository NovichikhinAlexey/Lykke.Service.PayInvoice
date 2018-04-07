using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IInvoiceService
    {
        Task<IReadOnlyList<Invoice>> GetAsync(string merchantId);

        Task<Invoice> GetAsync(string merchantId, string invoiceId);

        Task<Invoice> GetByIdAsync(string invoiceId);

        Task<IReadOnlyList<HistoryItem>> GetHistoryAsync(string invoiceId);

        Task<Invoice> CreateDraftAsync(Invoice invoice);

        Task UpdateDraftAsync(Invoice invoice);

        Task<Invoice> CreateAsync(Invoice invoice);

        Task<Invoice> CreateFromDraftAsync(string invoiceId);

        Task UpdateAsync(PaymentRequestDetailsMessage message);

        Task DeleteAsync(string invoiceId);
    }
}