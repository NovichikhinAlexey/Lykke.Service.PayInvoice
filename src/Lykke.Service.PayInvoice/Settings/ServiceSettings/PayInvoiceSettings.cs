﻿using Lykke.Service.PayInvoice.Core.Settings;
using Lykke.Service.PayInvoice.Settings.ServiceSettings.Db;
using Lykke.Service.PayInvoice.Settings.ServiceSettings.Rabbit;

namespace Lykke.Service.PayInvoice.Settings.ServiceSettings
{
    public class PayInvoiceSettings
    {
        public DbSettings Db { get; set; }
        public RabbitSettings Rabbit { get; set; }
        public RetryPolicySettings RetryPolicy { get; set; }
        public DistributedCacheSettings DistributedCacheSettings { get; set; }
        public CacheExpirationPeriodsSettings CacheExpirationPeriods { get; set; }
        public string PayInvoicePortalUrl { get; set; }
    }
}
