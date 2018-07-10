using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInvoice.Settings.ServiceSettings;
using Lykke.Service.PayInvoice.Settings.SlackNotifications;

namespace Lykke.Service.PayInvoice.Settings
{
    public class AppSettings
    {
        public PayInvoiceSettings PayInvoiceService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public PayInternalServiceClientSettings PayInternalServiceClient { get; set; }
        public PayHistory.Client.Publisher.RabbitMqPublisherSettings PayHistoryServicePublisher { get; set; }
        public PayCallback.Client.InvoiceConfirmation.RabbitMqPublisherSettings PayInvoiceConfirmationPublisher { get; set; }
        public PayPushNotifications.Client.Publisher.RabbitMqPublisherSettings PayPushNotificationsServicePublisher { get; set; }
    }
}
