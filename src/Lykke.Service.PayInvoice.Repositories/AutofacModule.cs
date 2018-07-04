using Autofac;
using AzureStorage;
using AzureStorage.Blob;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Common.Log;
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
        private readonly ILog _log;

        public AutofacModule(IReloadingManager<string> connectionString, ILog log)
        {
            _connectionString = connectionString;
            _log = log;
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

            builder.RegisterInstance<IFileRepository>(
                new FileRepository(AzureBlobStorage.Create(_connectionString)));

            builder.RegisterInstance<IFileInfoRepository>(
                new FileInfoRepository(CreateTable<FileInfoEntity>(invoiceFilesTableName)));

            builder.RegisterInstance<IInvoiceRepository>(
                new InvoiceRepository(CreateTable<InvoiceEntity>(invoicesTableName),
                    CreateTable<AzureIndex>(invoicesTableName),
                    CreateTable<AzureIndex>(invoicesTableName)));

            builder.RegisterInstance<IEmployeeRepository>(
                new EmployeeRepository(CreateTable<EmployeeEntity>(employeesTableName),
                    CreateTable<AzureIndex>(employeesTableName)));

            builder.RegisterInstance<IHistoryRepository>(
                new HistoryRepository(CreateTable<HistoryItemEntity>(invoiceHistoryTableName)));

            builder.RegisterInstance<IMerchantSettingRepository>(
                new MerchantSettingRepository(CreateTable<MerchantSettingEntity>(merchantSettingTableName)));

            builder.RegisterInstance<IPaymentRequestHistoryRepository>(
                new PaymentRequestHistoryRepository(CreateTable<PaymentRequestHistoryItemEntity>(paymentRequestHistoryTableName)));

            builder.RegisterInstance<IInvoiceDisputeRepository>(
                new InvoiceDisputeRepository(CreateTable<InvoiceDisputeEntity>(invoiceDisputeTableName)));

            //InvoiceDisputeRepository
            builder.RegisterInstance<IInvoicePayerHistoryRepository>(
                new InvoicePayerHistoryRepository(CreateTable<InvoicePayerHistoryEntity>(invoicePayerHistoryTableName)));
        }

        private INoSQLTableStorage<T> CreateTable<T>(string name) where T : class, ITableEntity, new()
        {
            return AzureTableStorage<T>.Create(_connectionString, name, _log);
        }
    }
}
