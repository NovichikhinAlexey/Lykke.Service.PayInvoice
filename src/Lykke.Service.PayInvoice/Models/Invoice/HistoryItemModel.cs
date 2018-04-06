using System;
using System.Collections.Generic;
using Lykke.Service.PayInvoice.Core.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class HistoryItemModel
    {
        public string Id { get; set; }

        public string InvoiceId { get; set; }

        public string PaymentRequestId { get; set; }

        public string Number { get; set; }

        public string ClientName { get; set; }

        public string ClientEmail { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public InvoiceStatus Status { get; set; }

        public decimal PaymentAmount { get; set; }

        public decimal SettlementAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public string PaymentAssetId { get; set; }

        public string SettlementAssetId { get; set; }

        public decimal ExchangeRate { get; set; }

        public string WalletAddress { get; set; }

        public List<string> SourceWalletAddresses { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        public string ModifiedById { get; set; }
        
        public string Reason { get; set; }

        public DateTime Date { get; set; }
    }
}
