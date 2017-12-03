using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly INoSQLTableStorage<InvoiceEntity> _tableStorage;

        public InvoiceRepository(INoSQLTableStorage<InvoiceEntity> tableStorage)
        {
            this._tableStorage = tableStorage;
        }

        public async Task<bool> SaveInvoice(IInvoiceEntity invoice)
        {
            InvoiceRepository invoiceRepository = this;
            try
            {
                InvoiceEntity invoiceEntity = InvoiceEntity.Create(invoice);
               
                await invoiceRepository._tableStorage.InsertOrMergeAsync(invoiceEntity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<IInvoiceEntity>> GetInvoices()
        {
            return (await _tableStorage.GetDataAsync(InvoiceEntity.GeneratePartitionKey())).ToList<IInvoiceEntity>();
        }

        public async Task<IInvoiceEntity> GetInvoice(string invoiceId)
        {
            return await _tableStorage.GetDataAsync(InvoiceEntity.GeneratePartitionKey(), invoiceId);
        }
    }
}