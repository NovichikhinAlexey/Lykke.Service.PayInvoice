using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Clients
{
    public class ErrorResponseException : Exception
    {
        public ErrorResponseException()
        {
        }

        public ErrorResponseException(string message)
            : base(message)
        {
        }

        public ErrorResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ErrorResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
