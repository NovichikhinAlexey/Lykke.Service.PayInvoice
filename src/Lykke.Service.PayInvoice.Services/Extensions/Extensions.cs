using System.Collections.Generic;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Services.Extensions
{
    public static class Extensions
    {
        public static bool IsPaidStatus(this InvoiceStatus status)
        {
            var paidStatuses = new List<InvoiceStatus>() {
                    InvoiceStatus.Paid,
                    InvoiceStatus.Overpaid,
                    InvoiceStatus.Underpaid,
                    InvoiceStatus.LatePaid };
            return paidStatuses.Contains(status);
        }
    }
}
