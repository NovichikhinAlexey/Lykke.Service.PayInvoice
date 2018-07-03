using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain.InvoiceConfirmation;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IInvoiceConfirmationService
    {
        Task PublishInvoicePayment(InvoiceConfirmationCommand command);
        Task PublishDisputeRaised(DisputeRaisedConfirmationCommand command);
        Task PublishDisputeCancelled(DisputeCancelledConfirmationCommand command);
    }
}
