using Autofac;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Settings;

namespace Lykke.Service.PayInvoice.Services
{
    public class AutofacModule : Module
    {
        private readonly RetryPolicySettings _retryPolicySettings;

        public AutofacModule(
            RetryPolicySettings retryPolicySettings)
        {
            _retryPolicySettings = retryPolicySettings;
        }

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

            builder.RegisterType<MerchantService>()
                .As<IMerchantService>();

            builder.RegisterType<MerchantSettingService>()
                .As<IMerchantSettingService>();

            builder.RegisterType<HistoryOperationService>()
                .WithParameter(TypedParameter.From(_retryPolicySettings))
                .As<IHistoryOperationService>();

            builder.RegisterType<InvoiceConfirmationService>()
                .WithParameter(TypedParameter.From(_retryPolicySettings))
                .As<IInvoiceConfirmationService>();
        }
    }
}
