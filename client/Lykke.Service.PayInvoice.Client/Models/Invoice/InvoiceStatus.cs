namespace Lykke.Service.PayInvoice.Client.Models.Invoice
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