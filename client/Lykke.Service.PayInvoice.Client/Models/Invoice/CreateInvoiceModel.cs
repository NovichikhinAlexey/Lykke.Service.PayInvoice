using System;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    /// <summary>
    /// Represents an invoice creation details.
    /// </summary>
    public class CreateInvoiceModel
    {
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
        /// Gets or sets invoice due date.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The additional information.
        /// </summary>
        public string Note { get; set; }
    }
}
