using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Lykke.Service.PayInvoice.Validation;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class PayInvoicesRequest : GetSumToPayInvoicesRequest
    {
        [GreaterThan(0)]
        public decimal Amount { get; set; }
    }
}
