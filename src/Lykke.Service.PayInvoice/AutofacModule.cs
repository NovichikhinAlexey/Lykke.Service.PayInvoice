using Autofac;
using Common;
using Common.Log;
using Lykke.Service.PayCallback.Client;
using Lykke.Service.PayHistory.Client;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInvoice.Rabbit.Subscribers;
using Lykke.Service.PayInvoice.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.PayInvoice
{
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;

        public AutofacModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();
            
            builder.RegisterInstance(new PayInternalClient(_settings.CurrentValue.PayInternalServiceClient))
                .As<IPayInternalClient>()
                .SingleInstance();

            builder.RegisterHistoryOperationPublisher(_settings.CurrentValue.PayHistoryServicePublisher, _log);

            builder.RegisterInvoiceConfirmationPublisher(_settings.CurrentValue.InvoiceConfirmationPublisher, _log);

            builder.RegisterType<PaymentRequestSubscriber>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.PayInvoiceService.Rabbit));
        }
    }
}
