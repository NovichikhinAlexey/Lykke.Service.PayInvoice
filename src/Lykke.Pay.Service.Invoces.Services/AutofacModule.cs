using Autofac;
using Lykke.Pay.Service.Invoces.Core.Services;

namespace Lykke.Pay.Service.Invoces.Services
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterType<InvoiceService>()
                .As<IInvoiceService>();

            builder.RegisterType<FileService>()
                .As<IFileService>();
        }
    }
}
