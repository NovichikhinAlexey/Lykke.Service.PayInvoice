using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IInvoiceService
    {
        Task<IReadOnlyList<Invoice>> GetAsync(string merchantId);
        Task<bool> HasAnyInvoiceAsync(string merchantId);

        Task<Invoice> GetAsync(string merchantId, string invoiceId);

        Task<Invoice> GetByIdAsync(string invoiceId);

        Task<IReadOnlyList<Invoice>> GetByIdsAsync(string merchantId, IEnumerable<string> invoiceIds);

        Task<IReadOnlyList<Invoice>> GetByIdsAsync(IEnumerable<string> invoiceIds);

        Task<IReadOnlyList<Invoice>> GetByFilterAsync(InvoiceFilter invoiceFilter);

        Task<IReadOnlyList<HistoryItem>> GetHistoryAsync(string invoiceId);

        Task<IReadOnlyList<PaymentRequestHistoryItem>> GetPaymentRequestsOfInvoiceAsync(string invoiceId);

        Task<Invoice> CreateDraftAsync(Invoice invoice);

        Task UpdateDraftAsync(Invoice invoice);

        Task<Invoice> CreateAsync(Invoice invoice);

        Task<Invoice> ChangePaymentRequestAsync(string invoiceId, string paymentAssetId);

        Task<Invoice> CreateFromDraftAsync(string invoiceId);

        Task UpdateAsync(PaymentRequestDetailsMessage message);

        Task DeleteAsync(string invoiceId);

        Task<IReadOnlyList<Invoice>> ValidateForPayingInvoicesAsync(string merchantId, IEnumerable<string> invoicesIds, string assetForPay);

        Task PayInvoicesAsync(Employee employee, IEnumerable<Invoice> invoices, string assetForPay, decimal amount);

        Task<decimal> GetSumInAssetForPayAsync(string merchantId, IEnumerable<Invoice> invoices, string assetForPay);

        Task MarkDisputeAsync(string invoiceId, string reason, string employeeId);

        Task CancelDisputeAsync(string invoiceId, string employeeId);

        Task<InvoiceDispute> GetInvoiceDisputeAsync(string invoiceId);
    }
}
