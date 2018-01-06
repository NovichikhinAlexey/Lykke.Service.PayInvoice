using Autofac;
using Lykke.Pay.Service.Invoces.Clients.LykkePay;
using Lykke.Pay.Service.Invoces.Core.Clients;
using Lykke.Pay.Service.Invoces.Core.Utils;
using Lykke.Pay.Service.Invoces.Settings;
using Lykke.Pay.Service.Invoces.Utils;
using Lykke.SettingsReader;

namespace Lykke.Pay.Service.Invoces
{
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<AppSettings> _setting;

        public AutofacModule(IReloadingManager<AppSettings> setting)
        {
            _setting = setting;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new LykkePayServiceClient(_setting.CurrentValue.LykkePayServiceClient))
                .As<ILykkePayServiceClient>()
                .SingleInstance();

            builder.RegisterInstance(new CallbackUrlFormatter(_setting.Nested(o => o.InvoicesService.CallbackHostUrl)))
                .As<ICallbackUrlFormatter>()
                .SingleInstance();
        }
    }
}
