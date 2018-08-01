using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using LykkePay.Common.Validation;

namespace Lykke.Service.PayInvoice.Models.MerchantSetting
{
    public class UpdateBaseAssetRequest
    {
        [Required]
        [RowKey]
        public string MerchantId { get; set; }
        [Required]
        public string BaseAsset { get; set; }
    }
}
