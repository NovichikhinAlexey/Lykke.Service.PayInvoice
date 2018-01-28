using System;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    public class CreateDraftInvoiceModel
    {
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public string SettlementAssetId { get; set; }
        public string PaymentAssetId { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string EmployeeId { get; set; }
        public DateTime DueDate { get; set; }
    }
}
