using Lykke.Service.PayInvoice.Core.Domain;
using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IMerchantSettingService
    {
        Task<MerchantSetting> GetByIdAsync(string merchantId);

        Task<string> GetBaseAssetAsync(string merchantId);

        Task<MerchantSetting> SetAsync(MerchantSetting merchantSettings);

        Task<string> GetBaseAssetByIdAsync(string merchantId);

        Task SetBaseAssetAsync(string merchantId, string baseAsset);
    }
}
