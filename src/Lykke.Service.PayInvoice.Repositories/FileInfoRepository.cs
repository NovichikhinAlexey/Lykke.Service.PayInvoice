using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Repositories;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class FileInfoRepository : IFileInfoRepository
    {
        private readonly INoSQLTableStorage<FileInfoEntity> _storage;

        public FileInfoRepository(INoSQLTableStorage<FileInfoEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<IFileInfo>> GetAsync(string invoiceId)
        {
            IEnumerable<FileInfoEntity> entities = await _storage.GetDataAsync(GetPartitionKey(invoiceId));

            return Mapper.Map<List<FileInfo>>(entities);
        }

        public async Task<IFileInfo> GetAsync(string invoiceId, string fileId)
        {
            FileInfoEntity entity = await _storage.GetDataAsync(GetPartitionKey(invoiceId), fileId);

            return Mapper.Map<FileInfo>(entity);
        }

        public async Task<string> InsertAsync(IFileInfo fileInfo)
        {
            var entity = new FileInfoEntity
            {
                RowKey = GetRowKey(),
                PartitionKey = GetPartitionKey(fileInfo.InvoiceId)
            };

            Mapper.Map(fileInfo, entity);

            await _storage.InsertAsync(entity);

            return entity.RowKey;
        }

        public async Task UpdateAsync(IFileInfo fileInfo)
        {
            await _storage.MergeAsync(GetPartitionKey(fileInfo.InvoiceId), fileInfo.FileId, entity =>
            {
                Mapper.Map(fileInfo, entity);
                return entity;
            });
        }

        public async Task DeleteAsync(string invoiceId)
        {
            IEnumerable<FileInfoEntity> entities = await _storage.GetDataAsync(GetPartitionKey(invoiceId));

            await _storage.DeleteAsync(entities);
        }

        public async Task DeleteAsync(string invoiceId, string fileId)
        {
            await _storage.DeleteAsync(GetPartitionKey(invoiceId), fileId);
        }

        private static string GetPartitionKey(string invoiceId)
            => invoiceId;

        private static string GetRowKey()
            => Guid.NewGuid().ToString("D");
    }
}
