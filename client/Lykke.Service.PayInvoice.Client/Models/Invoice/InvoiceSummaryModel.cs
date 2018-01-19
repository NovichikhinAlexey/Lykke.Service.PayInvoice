namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    public class InvoiceSummaryModel
    {
        public string InvoiceId { get; set; }
        public double InvoiceAmount { get; set; }
        public string InvoiceCurrency { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderId { get; set; }
        public double OrderAmount { get; set; }
        public string OrderCurrency { get; set; }
        public string ClientName { get; set; }
        public string Status { get; set; }
        public string WalletAddress { get; set; }
        public string TransactionTime { get; set; }
    }
}
