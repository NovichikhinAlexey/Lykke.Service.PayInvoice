namespace Lykke.Service.PayInvoice.Core.Domain
{
    public interface IFileInfo
    {
        string InvoiceId { get; set; }
        string FileId { get; set; }
        string FileName { get; set; }
        string FileMetaType { get; set; }
        int FileSize { get; set; }
    }
}