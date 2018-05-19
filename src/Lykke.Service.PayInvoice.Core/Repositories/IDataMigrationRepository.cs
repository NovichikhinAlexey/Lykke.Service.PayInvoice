using Lykke.Service.PayInvoice.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IDataMigrationRepository
    {
        Task<IReadOnlyList<string>> GetAllAsync();

        Task<bool> IsExistAsync(string migrationName);

        Task<bool> AddAsync(string migrationName);
    }
}
