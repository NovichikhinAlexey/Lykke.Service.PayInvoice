using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    /// <summary>
    /// Request to pay invoices
    /// </summary>
    public class PayInvoicesRequest
    {
        /// <summary>
        /// Gets or sets merchant id
        /// </summary>
        [Required]
        public string MerchantId { get; set; }
        /// <summary>
        /// Gets or sets invoices ids
        /// </summary>
        [Required]
        public IEnumerable<string> InvoicesIds { get; set; }
        /// <summary>
        /// Gets or sets amount
        /// </summary>
        [Required]
        public decimal Amount { get; set; }
    }
}
