using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lykke.Service.PayInvoice.Validation;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class GetSumToPayInvoicesRequest
    {
        [Required]
        [RowKey]
        public string EmployeeId { get; set; }
        [Required]
        [NotEmptyCollection]
        [RowKey]
        public IEnumerable<string> InvoicesIds { get; set; }
        /// <summary>
        /// Optional, if null then BaseAsset will be used
        /// </summary>
        public string AssetForPay { get; set; }
    }
}
