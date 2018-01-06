using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Core.Services
{
    public interface IFileService
    {
        Task<IEnumerable<IFileInfo>> GetInfoAsync(string invoiceId);
        Task<IFileInfo> GetInfoAsync(string invoiceId, string fileId);
        Task<byte[]> GetFileAsync(string fileId);
        Task SaveAsync(IFileInfo fileInfo, byte[] content);
    }
}
