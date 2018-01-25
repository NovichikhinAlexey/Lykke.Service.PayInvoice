﻿using Autofac;
using Common;
using Common.Log;
using Lykke.Service.Balances.Client;
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
            
            builder.RegisterInstance(new BalancesClient(_settings.CurrentValue.BalancesServiceClient.ServiceUrl, _log))
                .As<IBalancesClient>()
                .SingleInstance();
            
            builder.RegisterType<TransactionUpdatesSubscriber>()
                .AsSelf()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.PayInvoiceService.Rabbit));
        }
    }
}
