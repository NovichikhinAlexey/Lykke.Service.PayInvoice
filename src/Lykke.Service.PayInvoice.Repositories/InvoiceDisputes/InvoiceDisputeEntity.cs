using System;
using System.Collections.Generic;
using System.Text;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.PayInvoice.Repositories.InvoiceDisputes
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class InvoiceDisputeEntity : AzureTableEntity
    {
        private DateTime _createdAt;

        public InvoiceDisputeEntity()
        {
        }

        public InvoiceDisputeEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Id => RowKey;
        public string InvoiceId => PartitionKey;
        public string Reason { get; set; }
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
