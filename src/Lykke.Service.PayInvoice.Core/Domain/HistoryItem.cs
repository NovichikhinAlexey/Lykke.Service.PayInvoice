using System;
using System.Collections.Generic;
using Lykke.Service.PayInternal.Contract.PaymentRequest;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    /// <summary>
    /// Represents an invoice state.
    /// </summary>
    public class HistoryItem
    {
        /// <summary>
        /// The unique identified of state.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The related invoice id.
        /// </summary>
        public string InvoiceId { get; set; }

        /// <summary>
        /// The payment request id.
        /// </summary>
        public string PaymentRequestId { get; set; }

        /// <summary>
        /// The invoice number which provided when it created.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// The client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// The client email address.
        /// </summary>
        public string ClientEmail { get; set; }

        /// <summary>
        /// The invoice status.
        /// </summary>
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// The processing error of SettlementError
        /// </summary>
        public PaymentRequestProcessingError ProcessingError { get; set; }

        /// <summary>
        /// The invoice payment amount.
        /// </summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>
        /// The invoice settlement amount.
        /// </summary>
        public decimal SettlementAmount { get; set; }

        /// <summary>
        /// The invoice paid amount.
        /// </summary>
        public decimal PaidAmount { get; set; }

        /// <summary>
        /// The payment asset id.
        /// </summary>
        public string PaymentAssetId { get; set; }

        /// <summary>
        /// The settlement asset id.
        /// </summary>
        public string SettlementAssetId { get; set; }

        /// <summary>
        /// The payment request order exchange rate.
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// The destination wallet address.
        /// </summary>
        public string WalletAddress { get; set; }

        /// <summary>
        /// A collection of source wallet address.
        /// </summary>
        public List<string> SourceWalletAddresses { get; set; }

        /// <summary>
        /// The wallet address for refund.
        /// </summary>
        public string RefundWalletAddress { get; set; }

        /// <summary>
        /// The refund amount.
        /// </summary>
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// The invoice due date in UTC.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The invoice paid date in UTC.
        /// </summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// The employee id that was changed invoice or <c>null</c> if invoice was changed by payment system.
        /// </summary>
        public string ModifiedById { get; set; }

        /// <summary>
        /// The reason for changes.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// The changes date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The Billing Category
        /// </summary>
        public string BillingCategory { get; set; }

        /// <summary>
        /// The dispute attribute
        /// </summary>
        public bool Dispute { get; set; }
    }
}
