using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Repositories
{
    public class FileEntity : IFileEntity
    {
        public FileEntity()
        {
            
        }


        public FileEntity(IFileEntity entity) : this((IFileMetaEntity)entity)
        {
            FileBodyBase64 = entity.FileBodyBase64;
        }

        public FileEntity(IFileMetaEntity entity)
        {
            InvoiceId = entity.InvoiceId;
            FileId = entity.FileId;
            FileName = entity.FileName;
            FileMetaType = entity.FileMetaType;
            FileSize = entity.FileSize;
        }


        public string InvoiceId { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FileMetaType { get; set; }
        public int FileSize { get; set; }
        public string FileBodyBase64 { get; set; }
    }
}
