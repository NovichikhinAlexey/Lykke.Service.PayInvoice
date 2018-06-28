using System;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Common;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories.InvoiceDisputes
{
    public class InvoiceDisputeRepository : IInvoiceDisputeRepository
    {
        private INoSQLTableStorage<InvoiceDisputeEntity> _storage;

        private static string GetPartitionKey(string invoiceId)
            => invoiceId;

        private static string GetRowKey(DateTime creationDateTime)
            => IdGenerator.GenerateDateTimeIdNewFirst(creationDateTime);

        public InvoiceDisputeRepository(INoSQLTableStorage<InvoiceDisputeEntity> storage)
        {
            _storage = storage;
        }

        public async Task<InvoiceDispute> GetByInvoiceIdAsync(string invoiceId)
        {
            var entity = await _storage.GetTopRecordAsync(GetPartitionKey(invoiceId));
            return Mapper.Map<InvoiceDispute>(entity);
        }

        public async Task InsertAsync(InvoiceDispute invoiceDispute)
        {
            var entity = new InvoiceDisputeEntity(GetPartitionKey(invoiceDispute.InvoiceId), GetRowKey(invoiceDispute.CreatedAt));
            Mapper.Map(invoiceDispute, entity);
            await _storage.InsertAsync(entity);
        }
    }
}
