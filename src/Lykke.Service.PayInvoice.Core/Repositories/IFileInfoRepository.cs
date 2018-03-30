using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IFileInfoRepository
    {
        Task<IReadOnlyList<FileInfo>> GetAsync(string invoiceId);

        Task<FileInfo> GetAsync(string invoiceId, string fileId);

        Task<string> InsertAsync(FileInfo fileInfo);

        Task UpdateAsync(FileInfo fileInfo);

        Task DeleteAsync(string invoiceId);

        Task DeleteAsync(string invoiceId, string fileId);
    }
}
