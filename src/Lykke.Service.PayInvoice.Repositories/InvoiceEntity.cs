using System;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class InvoiceEntity : AzureTableEntity
    {
        public InvoiceEntity()
        {
        }

        public InvoiceEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Number { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string SettlementAssetId { get; set; }
        public string PaymentAssetId { get; set; }
        public string PaymentRequestId { get; set; }
        public string WalletAddress { get; set; }
        public string MerchantId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}