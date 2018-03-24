using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IFileService
    {
        Task<IEnumerable<FileInfo>> GetInfoAsync(string invoiceId);

        Task<FileInfo> GetInfoAsync(string invoiceId, string fileId);

        Task<byte[]> GetFileAsync(string fileId);

        Task SaveAsync(FileInfo fileInfo, byte[] content);

        Task DeleteAsync(string invoiceId, string fileId);
    }
}
