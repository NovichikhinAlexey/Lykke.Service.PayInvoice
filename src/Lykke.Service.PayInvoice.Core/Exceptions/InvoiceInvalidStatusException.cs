using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    [Serializable]
    internal class InvoiceInvalidStatusException : Exception
    {
        public InvoiceInvalidStatusException()
        {
        }

        public InvoiceInvalidStatusException(string message) : base(message)
        {
        }

        public InvoiceInvalidStatusException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvoiceInvalidStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
