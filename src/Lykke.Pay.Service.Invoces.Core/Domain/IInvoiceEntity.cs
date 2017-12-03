namespace Lykke.Pay.Service.Invoces.Core.Domain
{
    public interface IInvoiceEntity
    {
        string InvoiceId { get; set; }

        string InvoiceNumber { get; set; }

        double Amount { get; set; }

        string Currency { get; set; }

        string ClientId { get; set; }

        string ClientName { get; set; }

        string ClientUserId { get; set; }

        string ClientEmail { get; set; }

        string DueDate { get; set; }
    }
}