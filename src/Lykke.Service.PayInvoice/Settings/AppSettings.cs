using Lykke.Service.PayHistory.Client;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInvoice.Settings.Clients;
using Lykke.Service.PayInvoice.Settings.MonitoringService;
using Lykke.Service.PayInvoice.Settings.ServiceSettings;
using Lykke.Service.PayInvoice.Settings.SlackNotifications;
using Lykke.Service.PayMerchant.Client;

namespace Lykke.Service.PayInvoice.Settings
{
    public class AppSettings
    {
        public PayInvoiceSettings PayInvoiceService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public PayInternalServiceClientSettings PayInternalServiceClient { get; set; }
        public PayHistoryServiceClientSettings PayHistoryServiceClient { get; set; }
        public PayHistory.Client.Publisher.RabbitMqPublisherSettings PayHistoryServicePublisher { get; set; }
        public PayCallback.Client.InvoiceConfirmation.RabbitMqPublisherSettings PayInvoiceConfirmationPublisher { get; set; }
        public PayPushNotifications.Client.Publisher.RabbitMqPublisherSettings PayPushNotificationsServicePublisher { get; set; }
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
        public PayMerchantServiceClientSettings PayMerchantServiceClient { get; set; }
        public EmailPartnerRouterServiceClientSettings EmailPartnerRouterServiceClient { get; set; }
    }
}
