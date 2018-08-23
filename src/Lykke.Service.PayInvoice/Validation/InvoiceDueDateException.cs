using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Validation
{
    [Serializable]
    internal class InvoiceDueDateException : Exception
    {
        public InvoiceDueDateException() : base("DueDate must be greater than now.")
        {
        }

        public InvoiceDueDateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvoiceDueDateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
