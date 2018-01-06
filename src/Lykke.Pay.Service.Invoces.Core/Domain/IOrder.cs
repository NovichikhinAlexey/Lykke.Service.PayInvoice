namespace Lykke.Pay.Service.Invoces.Core.Domain
{
    public interface IOrder
    {
        string OrderId { get; }
        string Currency { get; }
        double Amount { get; }
        double TotalAmount { get; }
        double ExchangeRate { get; }
        string TransactionWaitingTime { get; }
    }
}
