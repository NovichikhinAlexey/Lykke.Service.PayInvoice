using System.Threading.Tasks;

namespace Lykke.Pay.Service.Invoces.Core.Repositories
{
    public interface IFileRepository
    {
        Task<byte[]> GetAsync(string id);
        Task<string> InsertAsync(byte[] file, string id);
        Task DeleteAsync(string id);
    }
}
