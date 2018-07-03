using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain.InvoiceConfirmation
{
    public class DisputeRaisedConfirmationCommand : DisputeCancelledConfirmationCommand
    {
        public string Reason { get; set; }
    }
}
