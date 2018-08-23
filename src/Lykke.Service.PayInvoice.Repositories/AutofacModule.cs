using Autofac;
using AzureStorage;
using AzureStorage.Blob;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Lykke.Common.Log;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Repositories.InvoiceDisputes;
using Lykke.Service.PayInvoice.Repositories.PaymentRequestHistory;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<string> _connectionString;

        public AutofacModule(IReloadingManager<string> connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            const string invoicesTableName = "Invoices";
            const string invoiceFilesTableName = "InvoiceFiles";
            const string employeesTableName = "Employees";
            const string invoiceHistoryTableName = "InvoiceHistory";
            const string merchantSettingTableName = "MerchantSettings";
            const string paymentRequestHistoryTableName = "PaymentRequestHistory";
            const string invoiceDisputeTableName = "InvoiceDispute";
            const string invoicePayerHistoryTableName = "InvoicePayerHistory";

            builder.Register(c =>
                new FileRepository(AzureBlobStorage.Create(_connectionString)))
                .As<IFileRepository>()
                .SingleInstance();

            builder.Register(c =>
                new FileInfoRepository(CreateTable<FileInfoEntity>(invoiceFilesTableName, c.Resolve<ILogFactory>())))
                .As<IFileInfoRepository>()
                .SingleInstance();

            builder.Register(c =>
                new InvoiceRepository(CreateTable<InvoiceEntity>(invoicesTableName, c.Resolve<ILogFactory>()),
                    CreateTable<AzureIndex>(invoicesTableName, c.Resolve<ILogFactory>()),
                    CreateTable<AzureIndex>(invoicesTableName, c.Resolve<ILogFactory>())))
                .As<IInvoiceRepository>()
                .SingleInstance();

            builder.Register(c =>
                new EmployeeRepository(CreateTable<EmployeeEntity>(employeesTableName, c.Resolve<ILogFactory>()),
                    CreateTable<AzureIndex>(employeesTableName, c.Resolve<ILogFactory>())))
                .As<IEmployeeRepository>()
                .SingleInstance();

            builder.Register(c =>
                new HistoryRepository(CreateTable<HistoryItemEntity>(invoiceHistoryTableName, c.Resolve<ILogFactory>())))
                .As<IHistoryRepository>()
                .SingleInstance();

            builder.Register(c =>
                new MerchantSettingRepository(CreateTable<MerchantSettingEntity>(merchantSettingTableName, c.Resolve<ILogFactory>())))
                .As<IMerchantSettingRepository>()
                .SingleInstance();

            builder.Register(c =>
                new PaymentRequestHistoryRepository(CreateTable<PaymentRequestHistoryItemEntity>(paymentRequestHistoryTableName, c.Resolve<ILogFactory>())))
                .As<IPaymentRequestHistoryRepository>()
                .SingleInstance();

            builder.Register(c =>
                new InvoiceDisputeRepository(CreateTable<InvoiceDisputeEntity>(invoiceDisputeTableName, c.Resolve<ILogFactory>())))
                .As<IInvoiceDisputeRepository>()
                .SingleInstance();

            builder.Register(c =>
                new InvoicePayerHistoryRepository(CreateTable<InvoicePayerHistoryEntity>(invoicePayerHistoryTableName, c.Resolve<ILogFactory>())))
                .As<IInvoicePayerHistoryRepository>()
                .SingleInstance();
        }

        private INoSQLTableStorage<T> CreateTable<T>(string name, ILogFactory logFactory) where T : class, ITableEntity, new()
        {
            return AzureTableStorage<T>.Create(_connectionString, name, logFactory);
        }
    }
}
