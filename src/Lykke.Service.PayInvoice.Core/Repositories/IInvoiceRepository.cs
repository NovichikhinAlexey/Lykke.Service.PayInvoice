using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IInvoiceRepository
    {
        Task<IReadOnlyList<Invoice>> GetAsync();

        Task<IReadOnlyList<Invoice>> GetAsync(string merchantId);

        Task<Invoice> GetAsync(string merchantId, string invoiceId);

        Task<Invoice> FindByIdAsync(string invoiceId);

        Task<Invoice> FindByPaymentRequestIdAsync(string paymentRequestId);

        Task<Invoice> InsertAsync(Invoice invoice);

        Task UpdateAsync(Invoice invoice);

        Task SetStatusAsync(string merchantId, string invoiceId, InvoiceStatus status);

        Task SetPaidAmountAsync(string merchantId, string invoiceId, decimal paidAmount);

        Task DeleteAsync(string merchantId, string invoiceId);
    }
}