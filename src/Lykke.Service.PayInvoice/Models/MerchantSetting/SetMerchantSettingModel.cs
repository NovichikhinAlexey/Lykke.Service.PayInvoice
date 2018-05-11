using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.MerchantSetting
{
    public class SetMerchantSettingModel
    {
        [Required]
        public string MerchantId { get; set; }

        public string BaseAsset { get; set; }
    }
}
