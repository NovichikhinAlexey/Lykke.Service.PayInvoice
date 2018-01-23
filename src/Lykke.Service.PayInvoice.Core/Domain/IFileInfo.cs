namespace Lykke.Service.PayInvoice.Core.Domain
{
    /// <summary>
    /// Represent a file meta data.
    /// </summary>
    public interface IFileInfo
    {
        /// <summary>
        /// The identifier of the invoice.
        /// </summary>
        string InvoiceId { get; }
        
        /// <summary>
        /// The unique identified of the file.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// The name of the file with extension.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// The file mime type.
        /// </summary>
        string Type { get; }
        
        /// <summary>
        /// The size, in bytes, of the file.
        /// </summary>
        int Size { get; }
    }
}