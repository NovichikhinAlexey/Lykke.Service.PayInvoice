using System;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    public interface IInvoiceDetails : IInvoice
    {
        string OrderId { get; }
        double ExchangeAmount { get; }
        DateTime OrderDueDate { get; }
    }
}