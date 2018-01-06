using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Core.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Pay.Service.Invoces.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly INoSQLTableStorage<InvoiceEntity> _storage;

        public InvoiceRepository(INoSQLTableStorage<InvoiceEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<IInvoice>> GetAsync()
        {
            IList<InvoiceEntity> entities = await _storage.GetDataAsync();

            return Mapper.Map<List<Invoice>>(entities);
        }

        // TODO: Need refactoring
        public async Task<IInvoice> GetAsync(string invoiceId)
        {
            string filter = TableQuery
                .GenerateFilterCondition(nameof(InvoiceEntity.RowKey), QueryComparisons.Equal, GetRowKey(invoiceId));

            var query = new TableQuery<InvoiceEntity>().Where(filter);

            IEnumerable<InvoiceEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<Invoice>(entities.FirstOrDefault());
        }

        public async Task<IInvoice> GetAsync(string merchantId, string invoiceId)
        {
            InvoiceEntity entity = await _storage.GetDataAsync(GetPartitionKey(merchantId), GetRowKey(invoiceId));

            return Mapper.Map<Invoice>(entity);
        }

        public async Task<IEnumerable<IInvoice>> GetByMerchantIdAsync(string merchantId)
        {
            IEnumerable<InvoiceEntity> entities = await _storage.GetDataAsync(GetPartitionKey(merchantId));

            return Mapper.Map<List<Invoice>>(entities);
        }

        // TODO: Need refactoring
        public async Task<IInvoice> GetByAddressAsync(string address)
        {
            string filter = TableQuery
                .GenerateFilterCondition(nameof(InvoiceEntity.WalletAddress), QueryComparisons.Equal, address);

            var query = new TableQuery<InvoiceEntity>().Where(filter);

            IEnumerable<InvoiceEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<Invoice>(entities.FirstOrDefault());
        }

        public async Task InsertAsync(IInvoice invoice)
        {
            var entity = new InvoiceEntity
            {
                PartitionKey = GetPartitionKey(invoice.MerchantId),
                RowKey = GetRowKey(invoice.InvoiceId)
            };

            Mapper.Map(invoice, entity);

            await _storage.InsertOrMergeAsync(entity);
        }

        public async Task UpdateAsync(IInvoice invoice)
        {
            await _storage.MergeAsync(GetPartitionKey(invoice.MerchantId), GetRowKey(invoice.InvoiceId),
                entity =>
                {
                    Mapper.Map(invoice, entity);
                    return entity;
                });
        }

        public async Task DeleteAsync(string merchantId, string invoiceId)
        {
            await _storage.DeleteAsync(GetPartitionKey(merchantId), GetRowKey(invoiceId));
        }

        private static string GetPartitionKey(string merchantId)
            => merchantId;

        private static string GetRowKey(string invoiceId)
            => invoiceId;
    }
}