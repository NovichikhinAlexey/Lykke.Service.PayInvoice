using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain.Email;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IEmailService
    {
        Task Send(PaymentReceivedEmail emailDetails);
    }
}
