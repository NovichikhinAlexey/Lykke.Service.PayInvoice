using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain.InvoicePayerHistory;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IInvoicePayerHistoryRepository
    {
        Task<InvoicePayerHistoryItem> GetAsync(string invoiceId, string paymentRequstId);
        Task<IReadOnlyList<InvoicePayerHistoryItem>> GetByInvoiceIdAsync(string invoiceId);
        Task InsertAsync(InvoicePayerHistoryItem historyItem);
    }
}
