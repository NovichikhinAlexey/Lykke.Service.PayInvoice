namespace Lykke.Service.PayInvoice.Core.Domain
{
    public class FileInfo : IFileInfo
    {
        public string InvoiceId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Size { get; set; }
    }
}
