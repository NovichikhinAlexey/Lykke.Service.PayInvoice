using System.ComponentModel.DataAnnotations;
using LykkePay.Common.Validation;

namespace Lykke.Service.PayInvoice.Models.MerchantSetting
{
    public class SetMerchantSettingModel
    {
        [Required]
        [RowKey]
        public string MerchantId { get; set; }
        [Required]
        public string BaseAsset { get; set; }
    }
}
