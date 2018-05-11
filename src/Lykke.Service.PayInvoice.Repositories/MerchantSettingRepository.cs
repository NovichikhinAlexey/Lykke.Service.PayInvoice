using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class MerchantSettingRepository : IMerchantSettingRepository
    {
        private readonly INoSQLTableStorage<MerchantSettingEntity> _storage;

        public MerchantSettingRepository(
            INoSQLTableStorage<MerchantSettingEntity> storage)
        {
            _storage = storage;
        }

        public async Task<MerchantSetting> GetByIdAsync(string merchantId)
        {
            MerchantSettingEntity entity = await _storage.GetDataAsync(GetPartitionKey(merchantId), GetRowKey(merchantId));

            return Mapper.Map<MerchantSetting>(entity);
        }
        
        public async Task<MerchantSetting> SetAsync(MerchantSetting merchantSetting)
        {
            var entity = new MerchantSettingEntity(GetPartitionKey(merchantSetting.MerchantId), GetRowKey(merchantSetting.MerchantId));

            Mapper.Map(merchantSetting, entity);

            await _storage.InsertOrMergeAsync(entity);

            return merchantSetting;
        }
        
        private static string GetPartitionKey(string merchantId)
            => merchantId;

        private static string GetRowKey(string merchantId)
            => merchantId;
    }
}