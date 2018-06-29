using System;
using System.Collections.Generic;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.Serializers;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.PayInvoice.Repositories.PaymentRequestHistory
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class InvoicePayerHistoryEntity : AzureTableEntity
    {
        private DateTime _createdAt;

        public InvoicePayerHistoryEntity()
        {
        }

        public InvoicePayerHistoryEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string InvoiceId => PartitionKey;

        public string PaymentRequestId { get; set; }

        public string EmployeeId { get; set; }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                MarkValueTypePropertyAsDirty(nameof(CreatedAt));
            }
        }
    }
}
