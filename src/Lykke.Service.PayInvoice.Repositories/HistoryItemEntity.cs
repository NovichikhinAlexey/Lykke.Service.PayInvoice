using System;
using System.Collections.Generic;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.Serializers;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.PayInvoice.Repositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class HistoryItemEntity : AzureTableEntity
    {
        private decimal _paymentAmount;
        private decimal _settlementAmount;
        private decimal _paidAmount;
        private decimal _exchangeRate;
        private decimal _refundAmount;
        private DateTime _dueDate;
        private DateTime? _paidDate;
        private DateTime _date;
        private bool _dispute;

        public HistoryItemEntity()
        {
        }

        public HistoryItemEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string InvoiceId { get; set; }

        public string PaymentRequestId { get; set; }

        public string Number { get; set; }

        public string ClientName { get; set; }

        public string ClientEmail { get; set; }

        public string Status { get; set; }

        public decimal PaymentAmount
        {
            get => _paymentAmount;
            set
            {
                _paymentAmount = value;
                MarkValueTypePropertyAsDirty(nameof(PaymentAmount));
            }
        }

        public decimal SettlementAmount
        {
            get => _settlementAmount;
            set
            {
                _settlementAmount = value;
                MarkValueTypePropertyAsDirty(nameof(SettlementAmount));
            }
        }

        public decimal PaidAmount
        {
            get => _paidAmount;
            set
            {
                _paidAmount = value;
                MarkValueTypePropertyAsDirty(nameof(PaidAmount));
            }
        }

        public string PaymentAssetId { get; set; }

        public string SettlementAssetId { get; set; }

        public decimal ExchangeRate
        {
            get => _exchangeRate;
            set
            {
                _exchangeRate = value;
                MarkValueTypePropertyAsDirty(nameof(ExchangeRate));
            }
        }

        public string WalletAddress { get; set; }

        [JsonValueSerializer]
        public List<string> SourceWalletAddresses { get; set; }

        public string RefundWalletAddress { get; set; }

        public decimal RefundAmount
        {
            get => _refundAmount;
            set
            {
                _refundAmount = value;
                MarkValueTypePropertyAsDirty(nameof(RefundAmount));
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

        public DateTime? PaidDate
        {
            get => _paidDate;
            set
            {
                _paidDate = value;
                MarkValueTypePropertyAsDirty(nameof(PaidDate));
            }
        }

        public string ModifiedById { get; set; }

        public string Reason { get; set; }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                MarkValueTypePropertyAsDirty(nameof(Date));
            }
        }
        public string BillingCategory { get; set; }

        public bool Dispute
        {
            get => _dispute;

            set
            {
                _dispute = value;
                MarkValueTypePropertyAsDirty(nameof(Dispute));
            }
        }
    }
}
