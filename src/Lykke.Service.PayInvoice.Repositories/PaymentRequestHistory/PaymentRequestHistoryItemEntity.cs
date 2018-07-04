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
        private DateTime _createdAt;
        private bool _isPaid;

        public PaymentRequestHistoryItemEntity()
        {
        }

        public PaymentRequestHistoryItemEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string PaymentAssetId { get; set; }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                MarkValueTypePropertyAsDirty(nameof(CreatedAt));
            }
        }

        public bool IsPaid
        {
            get => _isPaid;
            set
            {
                _isPaid = value;
                MarkValueTypePropertyAsDirty(nameof(IsPaid));
            }
        }
    }
}
