using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.PayInvoice.Repositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.None)]
    public class DataMigrationEntity : AzureTableEntity
    {
       
        public DataMigrationEntity()
        {
        }

        public DataMigrationEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}