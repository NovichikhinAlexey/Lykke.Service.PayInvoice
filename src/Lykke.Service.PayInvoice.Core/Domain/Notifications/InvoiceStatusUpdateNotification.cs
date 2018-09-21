using System;

namespace Lykke.Service.PayInvoice.Core.Domain.Notifications
{
    public class InvoiceStatusUpdateNotification
    {
        public string PaymentRequestId { get; set; }

        public InvoiceStatus Status { get; set; }

        public decimal Amount { get; set; }

        public string AssetId { get; set; }

        public DateTime Date { get; set; }

        public string MerchantId { get; set; }
    }
}
