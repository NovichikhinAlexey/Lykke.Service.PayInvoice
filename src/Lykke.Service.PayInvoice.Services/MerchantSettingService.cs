using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;

namespace Lykke.Service.PayInvoice.Services
{
    public class MerchantSettingService : IMerchantSettingService
    {
        private readonly IMerchantSettingRepository _merchantSettingRepository;
        private readonly ILog _log;

        public MerchantSettingService(
            IMerchantSettingRepository merchantSettingRepository,
            ILog log)
        {
            _merchantSettingRepository = merchantSettingRepository;
            _log = log.CreateComponentScope(nameof(MerchantSettingService));
        }

        public Task<MerchantSetting> GetByIdAsync(string merchantId)
        {
            return _merchantSettingRepository.GetByIdAsync(merchantId);
        }

        public async Task<string> GetBaseAssetAsync(string merchantId)
        {
            var merchantSetting = await GetByIdAsync(merchantId);
            return merchantSetting?.BaseAsset;
        }

        public async Task<MerchantSetting> SetAsync(MerchantSetting merchantSettings)
        {
            await _merchantSettingRepository.SetAsync(merchantSettings);

            _log.WriteInfo(nameof(SetAsync), merchantSettings, "Employee updated.");

            return merchantSettings;
        }
    }
}