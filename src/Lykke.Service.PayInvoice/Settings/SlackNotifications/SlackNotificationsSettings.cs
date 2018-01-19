using Lykke.AzureQueueIntegration;

namespace Lykke.Service.PayInvoice.Settings.SlackNotifications
{
    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }
}
