namespace Lykke.Service.PayInvoice.Core.Domain.InvoiceUpdate
{
    public class InvoiceUpdateMessage
    {
        public string MerchantId { get; set; }
        public string InvoiceId { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
