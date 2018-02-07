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
        decimal Amount { get; }

        /// <summary>
        /// The invoice due date in UTC.
        /// </summary>
        DateTime DueDate { get; }

        /// <summary>
        /// The invoice status.
        /// </summary>
        InvoiceStatus Status { get; }

        /// <summary>
        /// Gets or sets the settlement asset id.
        /// </summary>
        string SettlementAssetId { get; }
        
        /// <summary>
        /// Gets or sets the payment asset id.
        /// </summary>
        string PaymentAssetId { get; }

        /// <summary>
        /// Gets or sets the payment request id.
        /// </summary>
        string PaymentRequestId { get; }
        
        /// <summary>
        /// Gets or sets the wallet address.
        /// </summary>
        string WalletAddress { get; }
        
        /// <summary>
        /// The identifier of merchant.
        /// </summary>
        string MerchantId { get; }
        
        /// <summary>
        /// The identifier of the merchant employee which created invoice.
        /// </summary>
        string EmployeeId { get; }
        
        /// <summary>
        /// The date of creation in UTC.
        /// </summary>
        DateTime CreatedDate { get; }
    }
}