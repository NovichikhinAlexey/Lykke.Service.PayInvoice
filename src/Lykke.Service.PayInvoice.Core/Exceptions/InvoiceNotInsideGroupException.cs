using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    [Serializable]
    public class InvoiceNotInsideGroupException : Exception
    {
        public InvoiceNotInsideGroupException()
        {
        }

        public InvoiceNotInsideGroupException(string invoiceId) : base($"Invoice {invoiceId} is not inside group")
        {
            InvoiceId = invoiceId;
        }

        public InvoiceNotInsideGroupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvoiceNotInsideGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string InvoiceId { get; }
    }
}
