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

        string Label { get; set; }

        string Status { get; set; }

        string WalletAddress { get; set; }

        string StartDate { get; set; }
    }
}