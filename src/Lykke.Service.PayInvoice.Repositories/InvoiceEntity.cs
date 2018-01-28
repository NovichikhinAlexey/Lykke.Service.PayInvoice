using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.PayInvoice.Repositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class InvoiceEntity : AzureTableEntity
    {
        private DateTime _dueDate;
        private decimal _amount;
        private DateTime _createdDate;
        
        public InvoiceEntity()
        {
        }

        public InvoiceEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Number { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        
        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                MarkValueTypePropertyAsDirty(nameof(Amount));
            }
        }
        
        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                MarkValueTypePropertyAsDirty(nameof(DueDate));
            }
        }
        
        public string Status { get; set; }
        
        public string SettlementAssetId { get; set; }
        
        public string PaymentAssetId { get; set; }
        
        public string PaymentRequestId { get; set; }
        
        public string WalletAddress { get; set; }
        
        public string MerchantId { get; set; }
        
        public string EmployeeId { get; set; }
        
        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                _createdDate = value;
                MarkValueTypePropertyAsDirty(nameof(CreatedDate));
            }
        }
    }
}