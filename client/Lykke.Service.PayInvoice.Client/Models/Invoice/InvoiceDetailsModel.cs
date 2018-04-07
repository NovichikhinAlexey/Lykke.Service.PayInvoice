using System;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    public class InvoiceDetailsModel
    {
        /// <summary>
        /// Gets or sets invoice id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets invoice number.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets client email.
        /// </summary>
        public string ClientEmail { get; set; }

        /// <summary>
        /// Gets or sets invoice amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets invoice due date.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets invoice status.
        /// </summary>
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets invoice settlement asset id.
        /// </summary>
        public string SettlementAssetId { get; set; }

        /// <summary>
        /// Gets or sets invoice payment asset id.
        /// </summary>
        public string PaymentAssetId { get; set; }

        /// <summary>
        /// Gets or sets invoice payment request id.
        /// </summary>
        public string PaymentRequestId { get; set; }

        /// <summary>
        /// Gets or sets merchant id.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets employee id created invoice.
        /// </summary>
        public string EmployeeId { get; set; }
        
        /// <summary>
        /// The additional information.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets payment amount.
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// Gets or sets order due date.
        /// </summary>
        public DateTime OrderDueDate { get; set; }

        /// <summary>
        /// Gets or sets order created date.
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

        /// <summary>
        /// The paid date.
        /// </summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// The paid amount.
        /// </summary>
        public decimal PaidAmount { get; set; }
    }
}
