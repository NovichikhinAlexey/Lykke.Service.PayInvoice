using Lykke.Pay.Service.Invoces.Core.Domain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Pay.Service.Invoces.Repositories
{
    public class InvoiceEntity : TableEntity, IInvoiceEntity
    {
        public static string GeneratePartitionKey()
        {
            return "I";
        }

        public static string GenerateRowKey(string id)
        {
            return id;
        }

        public static InvoiceEntity Create(IInvoiceEntity invoice)
        {
            return new InvoiceEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(invoice.InvoiceId),
                Currency = invoice.Currency,
                ClientId = invoice.ClientId,
                ClientName = invoice.ClientName,
                ClientUserId = invoice.ClientUserId,
                ClientEmail = invoice.ClientEmail,
                DueDate = invoice.DueDate,
                InvoiceNumber = invoice.InvoiceNumber,
                Amount = invoice.Amount,
                Label = invoice.Label
            };
        }

        public string InvoiceId
        {
            get => RowKey;
            set => RowKey = value;
        }

        public string InvoiceNumber { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientUserId { get; set; }
        public string ClientEmail { get; set; }
        public string DueDate { get; set; }
        public string Label { get; set; }
    }
}