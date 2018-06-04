using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories.PaymentRequestHistory
{
    public class PaymentRequestHistoryRepository : IPaymentRequestHistoryRepository
    {
        private readonly INoSQLTableStorage<PaymentRequestHistoryItemEntity> _storage;

        public PaymentRequestHistoryRepository(INoSQLTableStorage<PaymentRequestHistoryItemEntity> storage)
        {
            _storage = storage;
        }

        public async Task<PaymentRequestHistoryItem> GetAsync(string invoiceId, string paymentRequstId)
        {
            PaymentRequestHistoryItemEntity entities = 
                await _storage.GetDataAsync(GetPartitionKey(invoiceId), GetRowKey(paymentRequstId));

            return Mapper.Map<PaymentRequestHistoryItem>(entities);
        }

        public async Task<IReadOnlyList<PaymentRequestHistoryItem>> GetByInvoiceIdAsync(string invoiceId)
        {
            IEnumerable<PaymentRequestHistoryItemEntity> entities = await _storage.GetDataAsync(GetPartitionKey(invoiceId));

            return Mapper.Map<List<PaymentRequestHistoryItem>>(entities);
        }

        public async Task InsertAsync(PaymentRequestHistoryItem historyItem)
        {
            var entity = new PaymentRequestHistoryItemEntity(
                GetPartitionKey(historyItem.InvoiceId), GetRowKey(historyItem.PaymentRequestId));

            Mapper.Map(historyItem, entity);

            await _storage.InsertAsync(entity);
        }

        public async Task DeleteAsync(string invoiceId)
        {
            IEnumerable<PaymentRequestHistoryItemEntity> entities =
                await _storage.GetDataAsync(GetPartitionKey(invoiceId));

            await _storage.DeleteAsync(entities);
        }

        private static string GetPartitionKey(string invoiceId)
            => invoiceId;

        private static string GetRowKey(string paymentRequestId)
            => paymentRequestId;
    }
}
