using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lykke.Service.PayInvoice.Validation;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class GetSumToPayInvoicesRequest
    {
        [Required]
        [RowKey]
        public string MerchantId { get; set; }
        [Required]
        [NotEmptyCollection]
        [RowKey]
        public IEnumerable<string> InvoicesIds { get; set; }
        public string AssetForPay { get; set; }
    }
}
