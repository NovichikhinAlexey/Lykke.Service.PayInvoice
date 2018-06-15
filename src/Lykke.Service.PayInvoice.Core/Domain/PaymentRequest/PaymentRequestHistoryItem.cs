using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain.PaymentRequest
{
    /// <summary>
    /// Represents an payment requests of the invoice
    /// </summary>
    public class PaymentRequestHistoryItem
    {
        /// <summary>
        /// The invoice id
        /// </summary>
        public string InvoiceId { get; set; }
        /// <summary>
        /// The payment request id
        /// </summary>
        public string PaymentRequestId { get; set; }
        /// <summary>
        /// The payment asset id
        /// </summary>
        public string PaymentAssetId { get; set; }
        /// <summary>
        /// The payment request is paid or not
        /// </summary>
        public bool IsPaid { get; set; }
        /// <summary>
        /// The date when history item was written
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
