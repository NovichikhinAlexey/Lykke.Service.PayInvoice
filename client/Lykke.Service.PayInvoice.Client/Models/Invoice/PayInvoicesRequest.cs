using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    /// <summary>
    /// Request to pay invoices
    /// </summary>
    public class PayInvoicesRequest : GetSumToPayInvoicesRequest
    {
        /// <summary>
        /// Gets or sets amount
        /// </summary>
        [Required]
        public decimal Amount { get; set; }
    }
}
