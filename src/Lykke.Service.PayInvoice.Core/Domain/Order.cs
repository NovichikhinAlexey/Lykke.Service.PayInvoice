namespace Lykke.Service.PayInvoice.Core.Domain
{
    public class Order : IOrder
    {
        public string OrderId { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public double TotalAmount { get; set; }
        public double ExchangeRate { get; set; }
        public string TransactionWaitingTime { get; set; }
    }
}
