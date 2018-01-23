using Lykke.Service.PayInvoice.Settings.ServiceSettings.Db;
using Lykke.Service.PayInvoice.Settings.ServiceSettings.Rabbit;

namespace Lykke.Service.PayInvoice.Settings.ServiceSettings
{
    public class PayInvoiceSettings
    {
        public DbSettings Db { get; set; }

        public RabbitSettings Rabbit { get; set; }
    }
}
