namespace Lykke.Service.PayInvoice.Core.Domain
{
    public interface IInvoice
    {
        string InvoiceId { get; }

        string InvoiceNumber { get; }

        double Amount { get; }

        string Currency { get; }

        string ClientId { get; }

        string ClientName { get; }

        string ClientUserId { get; }

        string ClientEmail { get; }

        string DueDate { get; }

        string Label { get; }

        string Status { get; }

        string WalletAddress { get; }

        string StartDate { get; }

        string Transaction { get; }

        string MerchantId { get; }
    }
}