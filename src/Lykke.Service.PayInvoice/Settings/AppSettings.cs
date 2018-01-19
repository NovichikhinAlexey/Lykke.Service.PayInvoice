using Lykke.Service.PayInvoice.Clients.LykkePay;
using Lykke.Service.PayInvoice.Settings.ServiceSettings;
using Lykke.Service.PayInvoice.Settings.SlackNotifications;

namespace Lykke.Service.PayInvoice.Settings
{
    public class AppSettings
    {
        public InvoicesSettings InvoicesService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public LykkePayServiceClientSettings LykkePayServiceClient { get; set; }
    }
}
