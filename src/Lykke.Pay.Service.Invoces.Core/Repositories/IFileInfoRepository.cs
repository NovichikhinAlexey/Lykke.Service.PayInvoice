using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Core.Repositories
{
    public interface IFileInfoRepository
    {
        Task<IEnumerable<IFileInfo>> GetAsync(string invoiceId);
        Task<IFileInfo> GetAsync(string invoiceId, string fileId);
        Task<string> InsertAsync(IFileInfo fileInfo);
        Task UpdateAsync(IFileInfo fileInfo);
        Task DeleteAsync(string invoiceId);
        Task DeleteAsync(string invoiceId, string fileId);
    }
}
