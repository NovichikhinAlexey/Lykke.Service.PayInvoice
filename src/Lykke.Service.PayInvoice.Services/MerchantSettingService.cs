using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Exceptions;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayMerchant.Client;

namespace Lykke.Service.PayInvoice.Services
{
    public class MerchantSettingService : IMerchantSettingService
    {
        private readonly IMerchantSettingRepository _merchantSettingRepository;
        private readonly IPayInternalClient _payInternalClient;
        private readonly IPayMerchantClient _payMerchantClient;
        private readonly ILog _log;

        public MerchantSettingService(
            IMerchantSettingRepository merchantSettingRepository,
            IPayInternalClient payInternalClient,
            ILogFactory logFactory, 
            IPayMerchantClient payMerchantClient)
        {
            _merchantSettingRepository = merchantSettingRepository;
            _payMerchantClient = payMerchantClient;
            _payInternalClient = payInternalClient;
            _log = logFactory.CreateLog(this);
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
            var merchantSettings = await _merchantSettingRepository.GetByIdAsync(merchantId);
            return merchantSettings?.BaseAsset;
        }

        public async Task<MerchantSetting> SetAsync(MerchantSetting model)
        {
            try
            {
                await _payMerchantClient.Api.GetByIdAsync(model.MerchantId);
            }
            catch (ClientApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
            {
                throw new MerchantNotFoundException(model.MerchantId);
            }

            await ValidateAssetAsync(model.MerchantId, model.BaseAsset);

            await _merchantSettingRepository.SetAsync(model);

            _log.Info("Merchant settings updated.", model);

            return model;
        }

        public async Task<string> GetBaseAssetByIdAsync(string merchantId)
        {
            var merchantSettings = await GetByIdAsync(merchantId);

            return merchantSettings.BaseAsset;
        }

        private async Task ValidateAssetAsync(string merchantId, string baseAsset)
        {
            try
            {
                var settlementAssetsResponse = await _payInternalClient.GetAvailableSettlementAssetsAsync(merchantId);

                bool isValidAsset = settlementAssetsResponse.Assets.ToList().Contains(baseAsset);

                if (!isValidAsset)
                    throw new AssetNotAvailableForMerchantException();
            }
            catch (DefaultErrorResponseException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new AssetNotAvailableForMerchantException();
            }
        }
    }
}
