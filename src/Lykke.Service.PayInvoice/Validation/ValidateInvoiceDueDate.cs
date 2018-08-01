using System;
using Lykke.Service.PayInvoice.Models.Invoice;

namespace Lykke.Service.PayInvoice.Validation
{
    public static class ValidateInvoiceDueDate
    {
        public static void ValidateDueDate(this CreateInvoiceModel model)
        {
            // https://docs.microsoft.com/en-us/rest/api/storageservices/Understanding-the-Table-Service-Data-Model?redirectedfrom=MSDN#property-types
            if (model.DueDate <= DateTime.UtcNow)
                throw new InvoiceDueDateException();
        }
    }
}
