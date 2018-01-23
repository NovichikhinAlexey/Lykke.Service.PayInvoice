using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class InvoiceMerchantLinkRepository : IInvoiceMerchantLinkRepository
    {
        private readonly INoSQLTableStorage<InvoiceMerchantLinkEntity> _storage;

        public InvoiceMerchantLinkRepository(INoSQLTableStorage<InvoiceMerchantLinkEntity> storage)
        {
            _storage = storage;
        }

        public async Task<string> GetAsync(string invoiceId)
        {
            IEnumerable<InvoiceMerchantLinkEntity> entities
                = await _storage.GetDataAsync(GetPartitionKey(invoiceId));

            InvoiceMerchantLinkEntity entity = entities.FirstOrDefault();

            return entity?.MerchantId;
        }

        public async Task InsertAsync(string merchantId, string invoiceId)
        {
            var entity = new InvoiceMerchantLinkEntity(GetPartitionKey(invoiceId), GetRowKey())
            {
                MerchantId = merchantId
            };
            
            await _storage.InsertAsync(entity);
        }

        public async Task DeleteAsync(string invoiceId)
        {
            await _storage.DeleteAsync(GetPartitionKey(invoiceId), GetRowKey());
        }

        private static string GetPartitionKey(string invoiceId)
            => invoiceId;

        private static string GetRowKey()
            => "MerchantIndex";
    }
}