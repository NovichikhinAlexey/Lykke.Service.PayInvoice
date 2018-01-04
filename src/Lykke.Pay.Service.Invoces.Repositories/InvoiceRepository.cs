using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Common.RemoteUi;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly INoSQLTableStorage<InvoiceEntity> _tableStorage;
        private readonly INoSQLTableStorage<FileMetaEntity> _tableFileStorage;
        private readonly IBlobStorage _fileBlobStorage;
        private const string FileContainer = "invoicefiles";

        public InvoiceRepository(INoSQLTableStorage<InvoiceEntity> tableStorage, INoSQLTableStorage<FileMetaEntity> tableFileStorage,
            IBlobStorage fileBlobStorage)
        {
            _tableStorage = tableStorage;
            _tableFileStorage = tableFileStorage;
            _fileBlobStorage = fileBlobStorage;
        }

        public async Task<bool> SaveInvoice(IInvoiceEntity invoice)
        {
            InvoiceRepository invoiceRepository = this;
            try
            {
                InvoiceEntity invoiceEntity = InvoiceEntity.Create(invoice);

                await invoiceRepository._tableStorage.InsertOrMergeAsync(invoiceEntity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<IInvoiceEntity>> GetInvoices(string merchantId)
        {
            if (!string.IsNullOrEmpty(merchantId))
                return (await _tableStorage.GetDataAsync(InvoiceEntity.GeneratePartitionKey(merchantId))).ToList<IInvoiceEntity>();
            return (await _tableStorage.GetDataAsync()).ToList<IInvoiceEntity>();
        }

        public async Task<IInvoiceEntity> GetInvoice(string merchantId, string invoiceId)
        {
            if (string.IsNullOrEmpty(invoiceId))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(merchantId))
            {
                return await _tableStorage.GetDataAsync(InvoiceEntity.GeneratePartitionKey(merchantId), invoiceId);
            }

            var invoiceList = await _tableStorage.GetDataAsync();
            var invoice = invoiceList.FirstOrDefault(e => e.InvoiceId.Equals(invoiceId));
            return invoice;
        }

        public async Task<IInvoiceEntity> GetInvoiceByAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }
            return (from i in await _tableStorage.GetDataAsync()
                    where address.Equals(i.WalletAddress)
                    select i).FirstOrDefault();
        }

        public async Task DeleteInvoice(string merchantId, string invoiceId)
        {
            await _tableStorage.DeleteAsync(InvoiceEntity.GeneratePartitionKey(merchantId), invoiceId);
        }

        public async Task UploadFile(IFileEntity entity)
        {
            var fileInfo = new FileMetaEntity(entity) { FileId = Guid.NewGuid().ToString() };
            await _tableFileStorage.InsertOrMergeAsync(fileInfo);
            await _fileBlobStorage.CreateContainerIfNotExistsAsync(FileContainer);
            await _fileBlobStorage.SaveBlobAsync(FileContainer, fileInfo.FileId, Convert.FromBase64String(entity.FileBodyBase64));
        }


        public async Task<IFileEntity> GetFileEntity(string invoiceId, string fileId)
        {
            await _fileBlobStorage.CreateContainerIfNotExistsAsync(FileContainer);
            var item = await _tableFileStorage.GetDataAsync(invoiceId, fileId);
            if (item == null)
            {
                return null;
            }
            var result = new FileEntity(item);
            var stream = await _fileBlobStorage.GetAsync(FileContainer, result.FileId);
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                result.FileBodyBase64 = Convert.ToBase64String(ms.ToArray());
            }
            return result;
        }

        public async Task<List<IFileMetaEntity>> GetFileMeta(string invoiceId)
        {

            var result = from fmi in await _tableFileStorage.GetDataAsync(invoiceId)
                         select (IFileMetaEntity)fmi;
            return result.ToList();
        }

        public async Task DeleteFiles(string invoiceId)
        {
            await _fileBlobStorage.CreateContainerIfNotExistsAsync(FileContainer);
            var files = (await _tableFileStorage.GetDataAsync(invoiceId)).ToList();
            foreach (var f in files)
            {
                await _tableFileStorage.DeleteAsync(f);
                await _fileBlobStorage.DelBlobAsync(FileContainer, f.FileId);
            }
        }
    }
}