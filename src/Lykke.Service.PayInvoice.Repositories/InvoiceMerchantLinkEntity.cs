using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class InvoiceMerchantLinkEntity : TableEntity
    {
        public InvoiceMerchantLinkEntity()
        {
        }

        public InvoiceMerchantLinkEntity(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public string MerchantId;
    }
}