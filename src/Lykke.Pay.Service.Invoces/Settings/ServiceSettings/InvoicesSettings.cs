using Lykke.Pay.Service.Invoces.Settings.ServiceSettings.Db;

namespace Lykke.Pay.Service.Invoces.Settings.ServiceSettings
{
    public class InvoicesSettings
    {
        public DbSettings Db { get; set; }

        public string CallbackHostUrl { get; set; }
    }
}
