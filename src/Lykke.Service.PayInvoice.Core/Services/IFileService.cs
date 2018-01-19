using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IFileService
    {
        Task<IEnumerable<IFileInfo>> GetInfoAsync(string invoiceId);
        Task<IFileInfo> GetInfoAsync(string invoiceId, string fileId);
        Task<byte[]> GetFileAsync(string fileId);
        Task SaveAsync(IFileInfo fileInfo, byte[] content);
    }
}
