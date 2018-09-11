using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain.Notifications;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IInvoiceNotificationsService
    {
        Task NotifyStatusUpdateAsync(InvoiceStatusUpdateNotification notification);
    }
}
