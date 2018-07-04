using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    /// <summary>
    /// Request to get sum for paying invoices
    /// </summary>
    public class GetSumToPayInvoicesRequest
    {
        /// <summary>
        /// Gets or sets merchant id
        /// </summary>
        [Required]
        public string EmployeeId { get; set; }
        /// <summary>
        /// Gets or sets invoices ids
        /// </summary>
        [Required]
        public IEnumerable<string> InvoicesIds { get; set; }
        /// <summary>
        /// Optional, need to pass when can differ from BaseAsset
        /// </summary>
        public string AssetForPay { get; set; }
    }
}
