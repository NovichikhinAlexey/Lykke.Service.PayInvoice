using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Repositories.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly INoSQLTableStorage<HistoryItemEntity> _storage;
        private readonly HistoryItemEntity Entity = new HistoryItemEntity();

        public HistoryRepository(INoSQLTableStorage<HistoryItemEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyList<HistoryItem>> GetAll()
        {
            IEnumerable<HistoryItemEntity> entities = await _storage.GetDataAsync();

            return Mapper.Map<List<HistoryItem>>(entities);
        }

        public async Task<IReadOnlyList<HistoryItem>> GetAllPaidAsync()
        {
            var result = new List<HistoryItemEntity>();

            var filter = StatusEqual(InvoiceStatus.Paid)
                .Or(StatusEqual(InvoiceStatus.Overpaid))
                .Or(StatusEqual(InvoiceStatus.Underpaid))
                .Or(StatusEqual(InvoiceStatus.LatePaid));

            var tableQuery = new TableQuery<HistoryItemEntity>().Where(filter);

            await _storage.ExecuteAsync(tableQuery, r => result = r.ToList());

            return Mapper.Map<List<HistoryItem>>(result);

            string StatusEqual(InvoiceStatus status)
            {
                return nameof(Entity.Status).PropertyEqual(status.ToString());
            }
        }

        public async Task<IReadOnlyList<HistoryItem>> GetByInvoiceIdAsync(string invoiceId)
        {
            IEnumerable<HistoryItemEntity> entities = await _storage.GetDataAsync(GetPartitionKey(invoiceId));

            return Mapper.Map<List<HistoryItem>>(entities);
        }

        public async Task InsertAsync(HistoryItem historyItem)
        {
            var entity = new HistoryItemEntity(GetPartitionKey(historyItem.InvoiceId), CreateRowKey());

            Mapper.Map(historyItem, entity);

            await _storage.InsertAsync(entity);
        }

        public async Task DeleteAsync(string invoiceId)
        {
            IEnumerable<HistoryItemEntity> entities = await _storage.GetDataAsync(GetPartitionKey(invoiceId));

            await _storage.DeleteAsync(entities);
        }

        private static string GetPartitionKey(string invoiceId)
            => invoiceId;

        private static string CreateRowKey()
            => Guid.NewGuid().ToString("D");
    }
}
