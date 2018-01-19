namespace Lykke.Service.PayInvoice.Core.Clients
{
    public class OrderResponse
    {
        public string Timestamp { get; set; }
        public string Address { get; set; }
        public string OrderId { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public double RecommendedFee { get; set; }
        public double TotalAmount { get; set; }
        public double ExchangeRate { get; set; }
        public string OrderRequestId { get; set; }
        public string TransactionWaitingTime { get; set; }
        public string MerchantPayRequestStatus { get; set; }
    }
}
