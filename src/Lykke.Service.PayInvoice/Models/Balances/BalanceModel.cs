namespace Lykke.Service.PayInvoice.Models.Balances
{
    public class BalanceModel
    {
        public string AssetId { get; set; }

        public double? Balance { get; set; }

        public double? Reserved { get; set; }
    }
}