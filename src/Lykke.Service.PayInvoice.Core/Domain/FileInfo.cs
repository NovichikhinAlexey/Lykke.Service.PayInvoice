namespace Lykke.Service.PayInvoice.Core.Domain
{
    public class FileInfo : IFileInfo
    {
        public string FileId { get; set; }
        public string InvoiceId { get; set; }
        public string FileName { get; set; }
        public string FileMetaType { get; set; }
        public int FileSize { get; set; }
    }
}
