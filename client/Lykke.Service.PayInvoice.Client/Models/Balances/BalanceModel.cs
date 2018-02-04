namespace Lykke.Service.PayInvoice.Client.Models.Balances
{
    /// <summary>
    /// Represents a balance details.
    /// </summary>
    public class BalanceModel
    {
        /// <summary>
        /// Gets or sers an asset id.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// Gets or sets a balance value.
        /// </summary>
        public double? Balance { get; set; }

        /// <summary>
        /// Gets or sets a reserved value.
        /// </summary>
        public double? Reserved { get; set; }
    }
}