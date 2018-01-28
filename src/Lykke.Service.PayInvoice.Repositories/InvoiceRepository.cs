using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly INoSQLTableStorage<InvoiceEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _invoiceIdIndexStorage;
        private readonly INoSQLTableStorage<AzureIndex> _paymentRequestIdIndexStorage;

        public InvoiceRepository(
            INoSQLTableStorage<InvoiceEntity> storage,
            INoSQLTableStorage<AzureIndex> invoiceIdIndexStorage,
            INoSQLTableStorage<AzureIndex> paymentRequestIdIndexStorage)
        {
            _storage = storage;
            _invoiceIdIndexStorage = invoiceIdIndexStorage;
            _paymentRequestIdIndexStorage = paymentRequestIdIndexStorage;
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

        public async Task<IInvoice> FindByIdAsync(string invoiceId)
        {
            AzureIndex index =
                await _invoiceIdIndexStorage.GetDataAsync(GetInvoiceIdIndexPartitionKey(invoiceId), GetInvoiceIdIndexRowKey());

            if (index == null)
                return null;

            InvoiceEntity entity = await _storage.GetDataAsync(index);
            
            var invoice = Mapper.Map<Invoice>(entity);

            return invoice;
        }
        
        public async Task<IInvoice> FindByPaymentRequestIdAsync(string paymentRequestId)
        {
            AzureIndex index =
                await _paymentRequestIdIndexStorage.GetDataAsync(GetPaymentRequestIndexPartitionKey(paymentRequestId), GetPaymentRequestIndexRowKey());

            if (index == null)
                return null;

            InvoiceEntity entity = await _storage.GetDataAsync(index);
            
            var invoice = Mapper.Map<Invoice>(entity);

            return invoice;
        }
        
        public async Task<IInvoice> InsertAsync(IInvoice invoice)
        {
            var entity = new InvoiceEntity(GetPartitionKey(invoice.MerchantId), CreateRowKey());

            Mapper.Map(invoice, entity);

            await _storage.InsertAsync(entity);

            await InsertInvoiceIdIndexAsync(entity);
            await InsertPaymentRequestIdIndexAsync(entity);

            return Mapper.Map<Invoice>(entity);
        }

        public async Task ReplaceAsync(IInvoice invoice)
        {
            var entity = new InvoiceEntity(GetPartitionKey(invoice.MerchantId), GetRowKey(invoice.Id));

            Mapper.Map(invoice, entity);

            entity.ETag = "*";

            await _storage.ReplaceAsync(entity);
            
            await InsertPaymentRequestIdIndexAsync(entity);
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
            InvoiceEntity entity = await _storage.GetDataAsync(GetPartitionKey(merchantId), GetRowKey(invoiceId));
            
            if(entity == null)
                return;

            await _storage.DeleteAsync(entity);

            await DeleteInvoiceIdIndexAsync(entity);
            await DeletePaymentRequestIdIndexAsync(entity);
        }

        private async Task InsertInvoiceIdIndexAsync(InvoiceEntity entity)
        {
            var index = AzureIndex.Create(GetInvoiceIdIndexPartitionKey(entity.RowKey), GetInvoiceIdIndexRowKey(), entity);
            
            await _invoiceIdIndexStorage.InsertOrReplaceAsync(index);
        }
        
        private async Task InsertPaymentRequestIdIndexAsync(InvoiceEntity entity)
        {
            if (string.IsNullOrEmpty(entity.PaymentRequestId))
                return;
            
            var index = AzureIndex.Create(GetPaymentRequestIndexPartitionKey(entity.PaymentRequestId), GetPaymentRequestIndexRowKey(), entity);
            
            await _paymentRequestIdIndexStorage.InsertOrReplaceAsync(index);
        }

        private async Task DeleteInvoiceIdIndexAsync(InvoiceEntity entity)
        {
            await _invoiceIdIndexStorage.DeleteAsync(GetInvoiceIdIndexPartitionKey(entity.RowKey), GetInvoiceIdIndexRowKey());
        }

        private async Task DeletePaymentRequestIdIndexAsync(InvoiceEntity entity)
        {
            if (string.IsNullOrEmpty(entity.PaymentRequestId))
                return;
            
            await _paymentRequestIdIndexStorage.DeleteAsync(GetPaymentRequestIndexPartitionKey(entity.PaymentRequestId), GetPaymentRequestIndexRowKey());
        }
        
        private static string GetPartitionKey(string merchantId)
            => merchantId;

        private static string GetRowKey(string invoiceId)
            => invoiceId;
        
        private static string CreateRowKey()
            => Guid.NewGuid().ToString("D");
        
        private static string GetInvoiceIdIndexPartitionKey(string invoiceId)
            => invoiceId;

        private static string GetInvoiceIdIndexRowKey()
            => "InvoiceIdIndex";
        
        private static string GetPaymentRequestIndexPartitionKey(string paymentRequestId)
            => paymentRequestId;

        private static string GetPaymentRequestIndexRowKey()
            => "PaymentRequestIdIndex";
    }
}