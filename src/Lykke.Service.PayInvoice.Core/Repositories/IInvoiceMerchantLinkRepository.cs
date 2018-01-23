using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    /// <summary>
    /// Contains methods for work with merchant-invoice index.
    /// </summary>
    public interface IInvoiceMerchantLinkRepository
    {
        /// <summary>
        /// Returns a merchant id for invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The merchant id.</returns>
        Task<string> GetAsync(string invoiceId);
        
        /// <summary>
        /// Creates an index for invoice merchant.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        Task InsertAsync(string merchantId, string invoiceId);
        
        /// <summary>
        /// Deletes an index for invoice merchant.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        Task DeleteAsync(string invoiceId);
    }
}