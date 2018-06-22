using System;
using System.Runtime.Serialization;

namespace Lykke.Service.PayInvoice.Core.Exceptions
{
    [Serializable]
    public class AssetNotAvailableForMerchantException : Exception
    {
        public AssetNotAvailableForMerchantException() : base("Asset is not available for merchant")
        {
        }

        public AssetNotAvailableForMerchantException(string message) : base(message)
        {
        }

        public AssetNotAvailableForMerchantException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AssetNotAvailableForMerchantException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
