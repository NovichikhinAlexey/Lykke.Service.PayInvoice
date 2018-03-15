using System;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    /// <summary>
    /// Invoice extended information included payment request order details.
    /// </summary>
    public class InvoiceDetails : Invoice
    {
        /// <summary>
        /// The amount which includes all fees and fixed while order is not expired.
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// The order due date.
        /// </summary>
        public DateTime OrderDueDate { get; set; }

        /// <summary>
        /// The order create date.
        /// </summary>
        public DateTime OrderCreatedDate { get; set; }
    }
}