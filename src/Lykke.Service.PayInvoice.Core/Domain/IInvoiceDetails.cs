using System;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    public interface IInvoiceDetails : IInvoice
    {
        decimal PaymentAmount { get; }
        DateTime OrderDueDate { get; }
        DateTime OrderCreatedDate { get; }
    }
}