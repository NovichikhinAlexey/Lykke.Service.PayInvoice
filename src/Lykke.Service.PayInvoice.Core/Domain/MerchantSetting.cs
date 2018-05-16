namespace Lykke.Service.PayInvoice.Core.Domain
{
    /// <summary>
    /// Merchant setting for representation
    /// </summary>
    public class MerchantSetting
    {
        /// <summary>
        /// The merchant id
        /// </summary>
        public string MerchantId { get; set; }
        
        /// <summary>
        /// The base (accounting) asset
        /// </summary>
        public string BaseAsset { get; set; }
    }
}
