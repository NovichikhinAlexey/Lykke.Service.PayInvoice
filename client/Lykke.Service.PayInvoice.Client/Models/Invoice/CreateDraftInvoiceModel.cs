using System;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    public class CreateDraftInvoiceModel
    {
        public string Number { get; set; }
        public double Amount { get; set; }
        public string AssetId { get; set; }
        public string AssetPairId { get; set; }
        public string ExchangeAssetId { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string MerchantStaffId { get; set; }
        public DateTime DueDate { get; set; }
    }
}
