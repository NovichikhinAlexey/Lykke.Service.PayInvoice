using System;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    /// <summary>
    /// Represents an invoice update details.
    /// </summary>
    public class UpdateInvoiceModel
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
        /// Gets or sets invoice amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets invoice settlement asset id.
        /// </summary>
        public string SettlementAssetId { get; set; }

        /// <summary>
        /// Gets or sets invoice payment asset id.
        /// </summary>
        public string PaymentAssetId { get; set; }

        /// <summary>
        /// Gets or sets client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets client email.
        /// </summary>
        public string ClientEmail { get; set; }

        /// <summary>
        /// Gets or sets employee id created invoice.
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets merchant id.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets invoice due date.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The additional information.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// The Billing Category
        /// </summary>
        public string BillingCategory { get; set; }

        /// <summary>
        /// The dispute attribute
        /// </summary>
        public bool Dispute { get; set; }
    }
}
