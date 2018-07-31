using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LykkePay.Common.Validation;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class GetSumToPayInvoicesRequest
    {
        [Required]
        [Guid]
        public string EmployeeId { get; set; }
        [Required]
        [NotEmptyCollection]
        [Guid]
        public IEnumerable<string> InvoicesIds { get; set; }
        /// <summary>
        /// Optional, if null then BaseAsset will be used
        /// </summary>
        public string AssetForPay { get; set; }
    }
}
