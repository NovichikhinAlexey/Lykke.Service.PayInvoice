using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Lykke.Service.PayInvoice.Validation;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class PayInvoicesRequest
    {
        [Required]
        [RowKey]
        public string MerchantId { get; set; }
        [Required]
        [NotEmptyCollection]
        [RowKey]
        public IEnumerable<string> InvoicesIds { get; set; }
        [GreaterThan(0)]
        public decimal Amount { get; set; }
    }
}
