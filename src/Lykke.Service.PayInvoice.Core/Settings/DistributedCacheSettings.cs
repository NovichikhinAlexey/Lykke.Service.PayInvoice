namespace Lykke.Service.PayInvoice.Core.Settings
{
    public class DistributedCacheSettings
    {
        public string RedisConfiguration { get; set; }
        public string PaymentLocksCacheKeyPattern { get; set; }
    }
}
