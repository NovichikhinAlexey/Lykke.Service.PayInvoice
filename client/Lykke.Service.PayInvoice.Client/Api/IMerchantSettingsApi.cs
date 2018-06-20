using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.MerchantSetting;
using Lykke.Service.PayInvoice.Core.Domain;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IMerchantSettingsApi
    {
        [Get("/api/MerchantSettings/{merchantId}")]
        Task<MerchantSetting> GetByIdAsync(string merchantId);

        [Post("/api/MerchantSettings")]
        Task<MerchantSetting> SetAsync([Body]MerchantSetting model);

        [Get("/api/MerchantSettings/baseAsset/{merchantId}")]
        Task<string> GetBaseAssetByIdAsync(string merchantId);

        [Post("/api/MerchantSettings/baseAsset")]
        Task SetBaseAssetAsync([Body]UpdateBaseAssetRequest model);
    }
}
