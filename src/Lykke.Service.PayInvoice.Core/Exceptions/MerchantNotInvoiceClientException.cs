using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    [Serializable]
    public class MerchantNotInvoiceClientException : Exception
    {
        public MerchantNotInvoiceClientException()
        {
        }

        public MerchantNotInvoiceClientException(string invoiceId) : base($"Current merchant is not client of the invoice {invoiceId}")
        {
            InvoiceId = invoiceId;
        }

        public MerchantNotInvoiceClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MerchantNotInvoiceClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string InvoiceId { get; }
    }
}
