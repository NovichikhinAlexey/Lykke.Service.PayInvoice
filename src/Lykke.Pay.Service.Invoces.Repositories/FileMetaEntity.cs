using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Pay.Service.Invoces.Repositories
{
    public class FileMetaEntity : TableEntity, IFileMetaEntity
    {
        public FileMetaEntity()
        {
            
        }

        public FileMetaEntity(IFileMetaEntity fileMetaEntity)
        {
            InvoiceId = fileMetaEntity.InvoiceId;
            FileId = fileMetaEntity.FileId;
            FileName = fileMetaEntity.FileName;
            FileMetaType = fileMetaEntity.FileMetaType;
            FileSize = fileMetaEntity.FileSize;
        }

        public string PrimaryPartitionKey => InvoiceId;
        public string PrimaryRowKey => FileId;

        public string InvoiceId { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileMetaType { get; set; }
        public int FileSize { get; set; }
    }
}
