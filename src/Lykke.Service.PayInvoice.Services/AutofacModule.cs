using Autofac;
using Autofac.Core;
using Lykke.Service.PayInvoice.Core.Domain.DistributedCache;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Settings;
using StackExchange.Redis;

namespace Lykke.Service.PayInvoice.Services
{
    public class AutofacModule : Module
    {
        private readonly DistributedCacheSettings _distributedCacheSettings;
        private readonly RetryPolicySettings _retryPolicySettings;

        public AutofacModule(
            DistributedCacheSettings distributedCacheSettings,
            RetryPolicySettings retryPolicySettings)
        {
            _distributedCacheSettings = distributedCacheSettings;
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

            builder.Register(c => ConnectionMultiplexer.Connect(_distributedCacheSettings.RedisConfiguration))
                .As<IConnectionMultiplexer>()
                .SingleInstance();

            builder.RegisterType<RedisLocksService>()
                .WithParameter(TypedParameter.From(_distributedCacheSettings.PaymentLocksCacheKeyPattern))
                .Keyed<IDistributedLocksService>(DistributedLockPurpose.InternalPayment)
                .SingleInstance();

            builder.RegisterType<InvoiceService>()
                .As<IInvoiceService>()
                .WithParameter(new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDistributedLocksService) &&
                                 pi.Name == "paymentLocksService",
                    (pi, ctx) => ctx.ResolveKeyed<IDistributedLocksService>(DistributedLockPurpose.InternalPayment)));
        }
    }
}
