using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IFileInfoRepository
    {
        Task<IReadOnlyList<IFileInfo>> GetAsync(string invoiceId);
        Task<IFileInfo> GetAsync(string invoiceId, string fileId);
        Task<string> InsertAsync(IFileInfo fileInfo);
        Task ReplaceAsync(IFileInfo fileInfo);
        Task DeleteAsync(string invoiceId);
        Task DeleteAsync(string invoiceId, string fileId);
    }
}
