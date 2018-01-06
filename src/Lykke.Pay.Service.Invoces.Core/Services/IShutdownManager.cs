using System.Threading.Tasks;

namespace Lykke.Pay.Service.Invoces.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
