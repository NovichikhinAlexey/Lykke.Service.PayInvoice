using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Domain.DataMigrations
{
    public interface IDataMigrationOneDotSeven
    {
        string Name { get; }
        Task<bool> ExecuteAsync();
    }
}
