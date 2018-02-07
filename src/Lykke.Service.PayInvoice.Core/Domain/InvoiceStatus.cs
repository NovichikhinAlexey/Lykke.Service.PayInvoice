namespace Lykke.Service.PayInvoice.Core.Domain
{
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