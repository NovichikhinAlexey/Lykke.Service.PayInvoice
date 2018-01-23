using Autofac;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInvoice.Settings;
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
            builder.RegisterInstance(new PayInternalClient(_setting.CurrentValue.PayInternalServiceClient))
                .As<IPayInternalClient>()
                .SingleInstance();
        }
    }
}
