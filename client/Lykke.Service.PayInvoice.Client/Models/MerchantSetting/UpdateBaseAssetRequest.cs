using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Client.Models.MerchantSetting
{
    /// <summary>
    /// Represents update base asset request
    /// </summary>
    public class UpdateBaseAssetRequest
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
