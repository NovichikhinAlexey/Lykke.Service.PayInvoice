namespace Lykke.Pay.Service.Invoces.Core.Domain
{
    public class Invoice : IInvoice
    {
        public string InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientUserId { get; set; }
        public string ClientEmail { get; set; }
        public string DueDate { get; set; }
        public string Label { get; set; }
        public string Status { get; set; }
        public string WalletAddress { get; set; }
        public string StartDate { get; set; }
        public string Transaction { get; set; }
        public string MerchantId { get; set; }
    }
}
