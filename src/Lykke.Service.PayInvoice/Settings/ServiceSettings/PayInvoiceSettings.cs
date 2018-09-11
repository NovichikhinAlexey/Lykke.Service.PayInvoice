using Lykke.Common.Chaos;
using Lykke.Service.PayInvoice.Core.Settings;
using Lykke.Service.PayInvoice.Settings.ServiceSettings.Db;
using Lykke.Service.PayInvoice.Settings.ServiceSettings.Rabbit;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayInvoice.Settings.ServiceSettings
{
    public class PayInvoiceSettings
    {
        public DbSettings Db { get; set; }
        public RabbitSettings Rabbit { get; set; }
        public RetryPolicySettings RetryPolicy { get; set; }
        public DistributedCacheSettings DistributedCacheSettings { get; set; }
        public CacheExpirationPeriodsSettings CacheExpirationPeriods { get; set; }
        public CqrsSettings Cqrs { get; set; }
        public string PayInvoicePortalUrl { get; set; }
    }

    public class CqrsSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }

        [Optional]
        public ChaosSettings ChaosKitty { get; set; }
    }
}
