namespace Lykke.Pay.Service.Invoces.Client.Models.File
{
    public class FileInfoModel
    {
        public string FileId { get; set; }
        public string InvoiceId { get; set; }
        public string FileName { get; set; }
        public string FileMetaType { get; set; }
        public int FileSize { get; set; }
    }
}
