using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly INoSQLTableStorage<InvoiceEntity> _storage;

        public InvoiceRepository(INoSQLTableStorage<InvoiceEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyList<IInvoice>> GetAsync()
        {
            IList<InvoiceEntity> entities = await _storage.GetDataAsync();

            return Mapper.Map<List<Invoice>>(entities);
        }

        public async Task<IReadOnlyList<IInvoice>> GetAsync(string merchantId)
        {
            IEnumerable<InvoiceEntity> entities = await _storage.GetDataAsync(GetPartitionKey(merchantId));

            return Mapper.Map<List<Invoice>>(entities);
        }
        
        public async Task<IInvoice> GetAsync(string merchantId, string invoiceId)
        {
            InvoiceEntity entity = await _storage.GetDataAsync(GetPartitionKey(merchantId), GetRowKey(invoiceId));

            return Mapper.Map<Invoice>(entity);
        }

        public async Task InsertAsync(IInvoice invoice)
        {
            var entity = new InvoiceEntity(GetPartitionKey(invoice.MerchantId), CreateRowKey());

            Mapper.Map(invoice, entity);

            await _storage.InsertAsync(entity);
        }

        public async Task ReplaceAsync(IInvoice invoice)
        {
            var entity = new InvoiceEntity(GetPartitionKey(invoice.MerchantId), GetRowKey(invoice.Id));

            Mapper.Map(invoice, entity);

            await _storage.ReplaceAsync(entity);
        }

        public async Task SetStatusAsync(string merchantId, string invoiceId, InvoiceStatus status)
        {
            await _storage.MergeAsync(GetPartitionKey(merchantId), GetRowKey(invoiceId), entity =>
            {
                entity.Status = status.ToString();
                return entity;
            });
        }

        public async Task DeleteAsync(string merchantId, string invoiceId)
        {
            await _storage.DeleteAsync(GetPartitionKey(merchantId), invoiceId);
        }

        private static string GetPartitionKey(string merchantId)
            => merchantId;

        private static string GetRowKey(string invoiceId)
            => invoiceId;
        
        private static string CreateRowKey()
            => Guid.NewGuid().ToString("D");
    }
}