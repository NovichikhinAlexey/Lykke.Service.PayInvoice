using Autofac;
using AzureStorage;
using AzureStorage.Blob;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Common.Log;
using Lykke.Service.PayInvoice.Core.Repositories;
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
        }

        private INoSQLTableStorage<T> CreateTable<T>(string name) where T : class, ITableEntity, new()
        {
            return AzureTableStorage<T>.Create(_connectionString, name, _log);
        }
    }
}
