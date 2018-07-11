using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain.PushNotification;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IPushNotificationService
    {
        Task PublishInvoicePayment(InvoicePaidPushNotificationCommand command);
        Task PublishDisputeRaised(DisputeRaisedPushNotificationCommand command);
        Task PublishDisputeCancelled(DisputeCancelledPushNotificationCommand command);
    }
}
