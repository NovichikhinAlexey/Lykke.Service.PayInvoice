using Lykke.Service.PayInvoice.Core.Domain.DataMigrations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IDataMigrationService
    {
        Task<IReadOnlyList<string>> GetAll();

        Task<DataMigrationResult> ExecuteAsync(string migrationName);
    }
}
