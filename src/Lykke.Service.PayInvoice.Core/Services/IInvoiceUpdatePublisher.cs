using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain.InvoiceUpdate;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IInvoiceUpdatePublisher
    {
        Task PublishAsync(InvoiceUpdateMessage message);
    }
}
