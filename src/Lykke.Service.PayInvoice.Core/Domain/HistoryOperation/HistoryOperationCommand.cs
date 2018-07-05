using System;

namespace Lykke.Service.PayInvoice.Core.Domain.HistoryOperation
{
    public class HistoryOperationCommand
    {
        public string InvoiceId { get; set; }
        public string InvoiceStatus { get; set; }
        public string MerchantId { get; set; }
        public string OppositeMerchantId { get; set; }
        public string EmployeeEmail { get; set; }
        public decimal Amount { get; set; }
        public string AssetId { get; set; }
        public string TxHash { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
