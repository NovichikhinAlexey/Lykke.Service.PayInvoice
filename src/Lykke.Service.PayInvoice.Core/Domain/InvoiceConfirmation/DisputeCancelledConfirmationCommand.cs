using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain.InvoiceConfirmation
{
    public class DisputeCancelledConfirmationCommand
    {
        public string EmployeeEmail { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime DateTime { get; set; }
    }
}
