namespace Lykke.Service.PayInvoice.Contract.Invoice
{
    public class InvoiceUpdateMessage
    {
        public string MerchantId { get; set; }
        public string InvoiceId { get; set; }
        public string Status { get; set; }
    }
}
