using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Common;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.InvoicePayerHistory;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories.PaymentRequestHistory
{
    public class InvoicePayerHistoryRepository : IInvoicePayerHistoryRepository
    {
        private readonly INoSQLTableStorage<InvoicePayerHistoryEntity> _storage;

        public InvoicePayerHistoryRepository(INoSQLTableStorage<InvoicePayerHistoryEntity> storage)
        {
            _storage = storage;
        }

        public async Task<InvoicePayerHistoryItem> GetAsync(string invoiceId, string paymentRequstId)
        {
            var entities = await GetByInvoiceIdAsync(invoiceId);

            var entity = entities.ToList().FirstOrDefault(x => x.PaymentRequestId == paymentRequstId);

            return entity;
        }

        public async Task<IReadOnlyList<InvoicePayerHistoryItem>> GetByInvoiceIdAsync(string invoiceId)
        {
            IEnumerable<InvoicePayerHistoryEntity> entities = await _storage.GetDataAsync(GetPartitionKey(invoiceId));

            return Mapper.Map<List<InvoicePayerHistoryItem>>(entities);
        }

        public async Task InsertAsync(InvoicePayerHistoryItem historyItem)
        {
            var entity = new InvoicePayerHistoryEntity
            {
                PartitionKey = GetPartitionKey(historyItem.InvoiceId),
                RowKey = GetRowKey(historyItem.CreatedAt),
                PaymentRequestId = historyItem.PaymentRequestId,
                EmployeeId = historyItem.EmployeeId,
                OutgoingHistoryOperationId = historyItem.OutgoingHistoryOperationId,
                IncomingHistoryOperationId = historyItem.IncomingHistoryOperationId,
                CreatedAt = historyItem.CreatedAt
            };

            await _storage.InsertAsync(entity);
        }

        private static string GetPartitionKey(string invoiceId)
            => invoiceId;

        private static string GetRowKey(DateTime creationDateTime)
            => IdGenerator.GenerateDateTimeIdNewFirst(creationDateTime);
    }
}
