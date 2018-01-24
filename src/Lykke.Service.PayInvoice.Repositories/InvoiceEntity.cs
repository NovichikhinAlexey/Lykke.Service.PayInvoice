using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class InvoiceEntity : TableEntity
    {
        public InvoiceEntity()
        {
        }

        public InvoiceEntity(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public string Number { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string WalletAddress { get; set; }
        public string AssetId { get; set; }
        public string ExchangeAssetId { get; set; }
        public string MerchantId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}