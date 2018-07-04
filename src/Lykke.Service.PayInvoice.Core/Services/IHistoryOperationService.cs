using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain.HistoryOperation;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IHistoryOperationService
    {
        Task PublishOutgoingInvoicePayment(HistoryOperationCommand command);
        Task PublishIncomingInvoicePayment(HistoryOperationCommand command);
    }
}
