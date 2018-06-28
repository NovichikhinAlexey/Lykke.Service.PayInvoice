using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;

namespace Lykke.Service.PayInvoice.Services
{
    public class MerchantSettingService : IMerchantSettingService
    {
        private readonly IMerchantSettingRepository _merchantSettingRepository;
        private readonly IPayInternalClient _payInternalClient;
        private readonly ILog _log;

        public MerchantSettingService(
            IMerchantSettingRepository merchantSettingRepository,
            IPayInternalClient payInternalClient,
            ILog log)
        {
            _merchantSettingRepository = merchantSettingRepository;
            _payInternalClient = payInternalClient;
            _log = log.CreateComponentScope(nameof(MerchantSettingService));
        }

        public async Task<MerchantSetting> GetByIdAsync(string merchantId)
        {
            var merchantSettings = await _merchantSettingRepository.GetByIdAsync(merchantId);

            if (merchantSettings == null)
                throw new MerchantSettingNotFoundException(merchantId);

            return merchantSettings;
        }

        public async Task<string> GetBaseAssetAsync(string merchantId)
        {
            var merchantSetting = await GetByIdAsync(merchantId);
            return merchantSetting?.BaseAsset;
        }

        public async Task<MerchantSetting> SetAsync(MerchantSetting model)
        {
            await ValidateAssetAsync(model.MerchantId, model.BaseAsset);

            await _merchantSettingRepository.SetAsync(model);

            _log.WriteInfo(nameof(SetAsync), model, "Merchant settings updated.");

            return model;
        }

        public async Task<string> GetBaseAssetByIdAsync(string merchantId)
        {
            var merchantSettings = await GetByIdAsync(merchantId);

            return merchantSettings.BaseAsset;
        }

        private async Task ValidateAssetAsync(string merchantId, string baseAsset)
        {
            var settlementAssetsResponse = await _payInternalClient.GetAvailableSettlementAssetsAsync(merchantId);

            bool isValidAsset = settlementAssetsResponse.Assets.ToList().Contains(baseAsset);

            if (!isValidAsset)
                throw new AssetNotAvailableForMerchantException();
        }
    }
}
