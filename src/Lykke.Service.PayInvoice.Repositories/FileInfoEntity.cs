using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories
{
    public class FileInfoEntity : TableEntity
    {
        public string InvoiceId { get; set; }
        public string FileName { get; set; }
        public string FileMetaType { get; set; }
        public int FileSize { get; set; }
    }
}
