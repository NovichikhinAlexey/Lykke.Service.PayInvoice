using System;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    public class Invoice : IInvoice
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public double Amount { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public string WalletAddress { get; set; }
        public string AssetId { get; set; }
        public string AssetPairId { get; set; }
        public string ExchangeAssetId { get; set; }
        public string MerchantId { get; set; }
        public string MerchantStaffId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
