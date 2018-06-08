using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IPaymentRequestHistoryRepository
    {
        Task<PaymentRequestHistoryItem> GetAsync(string invoiceId, string paymentRequstId);
        Task<IReadOnlyList<PaymentRequestHistoryItem>> GetByInvoiceIdAsync(string invoiceId);
        Task InsertAsync(PaymentRequestHistoryItem historyItem);
        Task DeleteAsync(string invoiceId);
    }
}
