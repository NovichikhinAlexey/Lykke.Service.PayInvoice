using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.PayInvoice.Repositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class MerchantSettingEntity : AzureTableEntity
    {
        private string _baseAsset;
        
        public MerchantSettingEntity()
        {
        }

        public MerchantSettingEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
        
        public string BaseAsset
        {
            get => _baseAsset;
            set
            {
                _baseAsset = value;
                MarkValueTypePropertyAsDirty(nameof(BaseAsset));
            }
        }
    }
}