using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    [Serializable]
    public class InvoiceDisputeNotFoundException : Exception
    {
        public InvoiceDisputeNotFoundException()
        {
        }

        public InvoiceDisputeNotFoundException(string invoiceId) : base("Invoice's dispute information not found")
        {
            InvoiceId = invoiceId;
        }

        public InvoiceDisputeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvoiceDisputeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string InvoiceId { get; }
    }
}
