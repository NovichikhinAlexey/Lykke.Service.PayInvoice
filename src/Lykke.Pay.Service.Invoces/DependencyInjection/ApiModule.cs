using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Pay.Service.Invoces.Core;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Core.Services;
using Lykke.Pay.Service.Invoces.Repositories;
using Lykke.Pay.Service.Invoces.Services;

namespace Lykke.Pay.Service.Invoces.DependencyInjection
{
    public class ApiModule : Module
    {
        private readonly ApplicationSettings _settings;
        private readonly ILog _log;

        public ApiModule(ApplicationSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log).SingleInstance();

            builder.RegisterInstance(_settings).SingleInstance();
            builder.RegisterInstance(_settings.InvoicesService).SingleInstance();


            RegisterInvoices(builder);
            
        }

        private void RegisterInvoices(ContainerBuilder builder)
        {
            
            var invoiceRequestRepo =
                new InvoiceRepository(AzureTableStorage<InvoiceEntity>.Create(new StringSettingsManager(_settings.InvoicesService.DbConnectionString), "Invoices", _log));
            builder.RegisterInstance(invoiceRequestRepo).As<IInvoiceRepository>();
            builder.RegisterType<InvoiceService>().As<IInvoiceService<IInvoiceEntity>>();

        }

       
    }
}