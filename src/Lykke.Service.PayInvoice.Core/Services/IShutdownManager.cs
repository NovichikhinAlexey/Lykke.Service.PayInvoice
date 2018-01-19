using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
