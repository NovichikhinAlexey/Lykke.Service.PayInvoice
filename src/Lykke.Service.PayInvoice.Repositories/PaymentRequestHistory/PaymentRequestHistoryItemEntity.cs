using System;
using System.Collections.Generic;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.Serializers;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.PayInvoice.Repositories.PaymentRequestHistory
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class PaymentRequestHistoryItemEntity : AzureTableEntity
    {
        private DateTime _historyCreatedOn;

        public PaymentRequestHistoryItemEntity()
        {
        }

        public PaymentRequestHistoryItemEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string PaymentAssetId { get; set; }

        public DateTime HistoryCreatedOn
        {
            get => _historyCreatedOn;
            set
            {
                _historyCreatedOn = value;
                MarkValueTypePropertyAsDirty(nameof(HistoryCreatedOn));
            }
        }
    }
}
