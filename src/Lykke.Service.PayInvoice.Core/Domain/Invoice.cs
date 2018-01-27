using System;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    public class Invoice : IInvoice
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public string SettlementAssetId { get; set; }
        public string PaymentAssetId { get; set; }
        public string PaymentRequestId { get; set; }
        public string MerchantId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
