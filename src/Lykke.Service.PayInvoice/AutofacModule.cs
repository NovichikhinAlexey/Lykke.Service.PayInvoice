using Autofac;
using Lykke.Service.PayInvoice.Clients.LykkePay;
using Lykke.Service.PayInvoice.Core.Clients;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Settings;
using Lykke.Service.PayInvoice.Utils;
using Lykke.SettingsReader;

namespace Lykke.Service.PayInvoice
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
