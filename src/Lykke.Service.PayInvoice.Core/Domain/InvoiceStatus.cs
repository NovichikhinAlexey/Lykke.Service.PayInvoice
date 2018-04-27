namespace Lykke.Service.PayInvoice.Core.Domain
{
    /// <summary>
    /// Represents an invoice status.
    /// </summary>
    public enum InvoiceStatus
    {
        /// <summary>
        /// Unknown status.
        /// </summary>
        None,

        /// <summary>
        /// Draft invoice.
        /// </summary>
        Draft,

        /// <summary>
        /// Invoice is waiting for payment.
        /// </summary>
        Unpaid,

        /// <summary>
        /// Invoice removed.
        /// </summary>
        Removed,

        /// <summary>
        /// Payment is in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// Payment received.
        /// </summary>
        Paid,

        /// <summary>
        /// Payment amount is less than expected.
        /// </summary>
        Underpaid,

        /// <summary>
        /// Payment amount is more than expected.
        /// </summary>
        Overpaid,

        /// <summary>
        /// Payment received after order due date.
        /// </summary>
        LatePaid,

        /// <summary>
        /// Refund is in progress.
        /// </summary>
        RefundInProgress,

        /// <summary>
        /// Refund successfully completed.
        /// </summary>
        Refunded,

        /// <summary>
        /// Transfer has not been confirmed.
        /// </summary>
        NotConfirmed,

        /// <summary>
        /// An unexpected error occurred during processing invoice.
        /// </summary>
        InternalError,

        /// <summary>
        /// Payment due date expired and no payment transactions detected
        /// </summary>
        PastDue
    }
}