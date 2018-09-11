using Autofac;
using Common;
using Lykke.Service.EmailPartnerRouter.Client;
using Lykke.Service.PayCallback.Client;
using Lykke.Service.PayHistory.Client;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInvoice.Rabbit.Subscribers;
using Lykke.Service.PayInvoice.Settings;
using Lykke.Service.PayMerchant.Client;
using Lykke.Service.PayPushNotifications.Client;
using Lykke.SettingsReader;

namespace Lykke.Service.PayInvoice.Modules
{
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public AutofacModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new PayInternalClient(_settings.CurrentValue.PayInternalServiceClient))
                .As<IPayInternalClient>()
                .SingleInstance();

            builder.RegisterPayHistoryClient(_settings.CurrentValue.PayHistoryServiceClient.ServiceUrl);

            builder.RegisterHistoryOperationPublisher(_settings.CurrentValue.PayHistoryServicePublisher);

            builder.RegisterInvoiceConfirmationPublisher(_settings.CurrentValue.PayInvoiceConfirmationPublisher);

            builder.RegisterPayPushNotificationPublisher(_settings.CurrentValue.PayPushNotificationsServicePublisher);

            builder.RegisterType<InvoicePaymentRequestSubscriber>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.PayInvoiceService.Rabbit));

            builder.RegisterType<NotificationsPaymentRequestSubscriber>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.PayInvoiceService.Rabbit));

            builder.RegisterPayMerchantClient(_settings.CurrentValue.PayMerchantServiceClient, null);

            builder.RegisterInstance(new EmailPartnerRouterClient(_settings.CurrentValue.EmailPartnerRouterServiceClient.ServiceUrl))
                .As<IEmailPartnerRouterClient>()
                .SingleInstance();
        }
    }
}
