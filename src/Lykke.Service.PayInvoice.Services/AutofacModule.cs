using Autofac;
using Lykke.Service.PayInvoice.Core.Domain.DataMigrations;
using Lykke.Service.PayInvoice.Core.Services;

namespace Lykke.Service.PayInvoice.Services
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
            
            builder.RegisterType<EmployeeService>()
                .As<IEmployeeService>();

            builder.RegisterType<MerchantSettingService>()
                .As<IMerchantSettingService>();

            builder.RegisterType<DataMigrationService>()
                .As<IDataMigrationService>();

            // Migrations
            builder.RegisterType<DataMigrationOneDotSeven>()
                .As<IDataMigrationOneDotSeven>();
        }
    }
}
