using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain.InvoicePayerHistory
{
    public class InvoicePayerHistoryItem
    {
        public string InvoiceId { get; set; }
        public string PaymentRequestId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
