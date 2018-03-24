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

        /// <summary>
        /// The order delta spread percent.
        /// </summary>
        public double DeltaSpread { get; set; }

        /// <summary>
        /// The order markup percent.
        /// </summary>
        public double MarkupPercent { get; set; }

        /// <summary>
        /// The order exchange rate.
        /// </summary>
        public decimal ExchangeRate { get; set; }
    }
}