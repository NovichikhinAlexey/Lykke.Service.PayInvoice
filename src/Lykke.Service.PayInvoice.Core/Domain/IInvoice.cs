using System;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    /// <summary>
    /// Represents an invoice.
    /// </summary>
    public interface IInvoice
    {
        /// <summary>
        /// The unique identified of the invoice.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The invoice number which provided when it created.
        /// </summary>
        string Number { get; }

        /// <summary>
        /// The client name.
        /// </summary>
        string ClientName { get; }

        /// <summary>
        /// The client email address.
        /// </summary>
        string ClientEmail { get; }

        /// <summary>
        /// The invoice amount.
        /// </summary>
        double Amount { get; }

        /// <summary>
        /// The invoice due date in UTC.
        /// </summary>
        DateTime DueDate { get; }

        /// <summary>
        /// The invoice status.
        /// </summary>
        InvoiceStatus Status { get; }

        /// <summary>
        /// The wallet associated with invoice. 
        /// </summary>
        string WalletAddress { get; }

        /// <summary>
        /// The identifier of the requested asset.
        /// </summary>
        string AssetId { get; }
        
        /// <summary>
        /// The identifier of the asset pair.
        /// </summary>
        string AssetPairId { get; }

        /// <summary>
        /// The identifier of the exchange asset.
        /// </summary>
        string ExchangeAssetId { get; }

        /// <summary>
        /// The identifier of merchant.
        /// </summary>
        string MerchantId { get; }
        
        /// <summary>
        /// The identifier of the merchant staff which created invoice.
        /// </summary>
        string MerchantStaffId { get; }
        
        /// <summary>
        /// The date of creation in UTC.
        /// </summary>
        DateTime CreatedDate { get; }
    }
}