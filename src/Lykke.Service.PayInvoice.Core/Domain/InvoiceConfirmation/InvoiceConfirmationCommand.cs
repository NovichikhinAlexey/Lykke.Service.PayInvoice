using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain.InvoiceConfirmation
{
    public class InvoiceConfirmationCommand
    {
        public string EmployeeEmail { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? AmountLeftPaid { get; set; }
        public string TxHash { get; set; }
        public DateTime TxFirstSeen { get; set; }
    }
}
