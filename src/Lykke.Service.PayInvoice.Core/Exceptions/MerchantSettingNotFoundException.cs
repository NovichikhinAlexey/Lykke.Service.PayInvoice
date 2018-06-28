using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    [Serializable]
    public class MerchantSettingNotFoundException : Exception
    {
        public MerchantSettingNotFoundException()
        {
        }

        public MerchantSettingNotFoundException(string merchantId) : base("Merchant settings not found")
        {
            MerchantId = merchantId;
        }

        public MerchantSettingNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MerchantSettingNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string MerchantId { get; }
    }
}
