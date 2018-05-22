using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IHistoryRepository
    {
        Task<IReadOnlyList<HistoryItem>> GetAll();

        Task<IReadOnlyList<HistoryItem>> GetAllPaidAsync();

        Task<IReadOnlyList<HistoryItem>> GetByInvoiceIdAsync(string invoiceId);

        Task InsertAsync(HistoryItem historyItem);

        Task DeleteAsync(string invoiceId);
    }
}
