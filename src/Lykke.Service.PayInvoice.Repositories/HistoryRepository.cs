using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly INoSQLTableStorage<HistoryItemEntity> _storage;

        public HistoryRepository(INoSQLTableStorage<HistoryItemEntity> storage)
        {
            _storage = storage;
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
