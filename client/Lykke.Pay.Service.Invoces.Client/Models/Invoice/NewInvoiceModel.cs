namespace Lykke.Pay.Service.Invoces.Client.Models.Invoice
{
    public class NewInvoiceModel
    {
        public string MerchantId { get; set; }
        public string InvoiceNumber { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string DueDate { get; set; }
    }
}
