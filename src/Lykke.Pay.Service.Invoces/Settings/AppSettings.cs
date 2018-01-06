using Lykke.Pay.Service.Invoces.Clients.LykkePay;
using Lykke.Pay.Service.Invoces.Settings.ServiceSettings;
using Lykke.Pay.Service.Invoces.Settings.SlackNotifications;

namespace Lykke.Pay.Service.Invoces.Settings
{
    public class AppSettings
    {
        public InvoicesSettings InvoicesService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public LykkePayServiceClientSettings LykkePayServiceClient { get; set; }
    }
}
