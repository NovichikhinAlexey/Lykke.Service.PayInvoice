using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    [Serializable]
    public class MerchantGroupNotFoundException : Exception
    {
        public MerchantGroupNotFoundException()
        {
        }

        public MerchantGroupNotFoundException(string merchantId) : base("Merchant group not found")
        {
            MerchantId = merchantId;
        }

        public MerchantGroupNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MerchantGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string MerchantId { get; set; }
    }
}
