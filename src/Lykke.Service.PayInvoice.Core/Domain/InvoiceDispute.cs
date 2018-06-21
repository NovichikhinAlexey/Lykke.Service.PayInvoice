using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    public class InvoiceDispute
    {
        public string InvoiceId { get; set; }
        public string Reason { get; set; }
        public string EmployeeId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
