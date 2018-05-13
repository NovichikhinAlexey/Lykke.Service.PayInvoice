using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class DataMigrationRepository : IDataMigrationRepository
    {
        private readonly INoSQLTableStorage<DataMigrationEntity> _storage;
        private static string GetPartitionKey() => "PayInvoice";

        private static string GetRowKey(string migrationName) => migrationName;

        public DataMigrationRepository(
            INoSQLTableStorage<DataMigrationEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyList<string>> GetAllAsync()
        {
            IList<DataMigrationEntity> entities = await _storage.GetDataAsync();

            return entities.OrderBy(x => x.Timestamp).Select(x => x.PartitionKey).ToList();
        }

        public async Task<bool> IsExistAsync(string migrationName)
        {
            var entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey(migrationName));
            return entity != null;
        }

        public async Task<bool> AddAsync(string migrationName)
        {
            var entity = new DataMigrationEntity(GetPartitionKey(), GetRowKey(migrationName));
            return await _storage.TryInsertAsync(entity);
        }
    }
}