using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain.HistoryOperation;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IHistoryOperationService
    {
        Task<string> PublishOutgoingInvoicePayment(HistoryOperationCommand command);
        Task<string> PublishIncomingInvoicePayment(HistoryOperationCommand command);
        Task RemoveAsync(string id);
    }
}
