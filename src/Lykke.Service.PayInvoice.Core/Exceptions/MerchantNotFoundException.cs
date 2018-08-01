using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    [Serializable]
    public class MerchantNotFoundException : Exception
    {
        public MerchantNotFoundException() : base("Merchant not found")
        {
        }

        public MerchantNotFoundException(string merchantId) : base("Merchant not found")
        {
            MerchantId = merchantId;
        }

        public MerchantNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MerchantNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string MerchantId { get; }
    }
}
