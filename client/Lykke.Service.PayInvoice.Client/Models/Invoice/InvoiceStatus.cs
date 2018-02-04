namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    /// <summary>
    /// Invoice statuses
    /// </summary>
    public enum InvoiceStatus
    {
        Draft,
        Paid,
        Unpaid,
        Removed,
        Underpaid,
        Overpaid,
        LatePaid,
        InProgress,
    }
}