using Lykke.Service.PayInvoice.Settings.ServiceSettings.Db;

namespace Lykke.Service.PayInvoice.Settings.ServiceSettings
{
    public class InvoicesSettings
    {
        public DbSettings Db { get; set; }

        public string CallbackHostUrl { get; set; }
    }
}
