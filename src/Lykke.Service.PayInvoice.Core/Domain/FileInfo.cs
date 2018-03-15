namespace Lykke.Service.PayInvoice.Core.Domain
{
    /// <summary>
    /// Represent a file meta data.
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// The identifier of the invoice.
        /// </summary>
        public string InvoiceId { get; set; }

        /// <summary>
        /// The unique identified of the file.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the file with extension.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The file mime type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The size, in bytes, of the file.
        /// </summary>
        public int Size { get; set; }
    }
}
