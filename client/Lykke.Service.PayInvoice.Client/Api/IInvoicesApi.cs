using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IInvoicesApi
    {
        [Get("/api/invoices/{invoiceId}")]
        Task<InvoiceModel> GetAsync(string invoiceId);

        [Get("/api/invoices/filter")]
        Task<IReadOnlyList<InvoiceModel>> GetByFilter([Query(CollectionFormat.Multi)] IEnumerable<string> merchantIds, [Query(CollectionFormat.Multi)] IEnumerable<string> clientMerchantIds, [Query(CollectionFormat.Multi)] IEnumerable<string> statuses, bool? dispute, [Query(CollectionFormat.Multi)] IEnumerable<string> billingCategories, decimal? greaterThan, decimal? lessThan);

        [Get("/api/invoices/{invoiceId}/history")]
        Task<IReadOnlyList<HistoryItemModel>> GetHistoryAsync(string invoiceId);

        [Get("/api/invoices/{invoiceId}/paymentrequests")]
        Task<IReadOnlyList<PaymentRequestHistoryItem>> GetPaymentRequestsAsync(string invoiceId);

        [Post("/api/invoices")]
        Task<InvoiceModel> CreateAsync([Body] CreateInvoiceModel model);

        [Post("/api/invoices/{invoiceId}")]
        Task<InvoiceModel> CreateFromDraftAsync(string invoiceId);

        [Post("/api/invoices/{invoiceId}/{paymentAssetId}")]
        Task<InvoiceModel> ChangePaymentAssetAsync(string invoiceId, string paymentAssetId);

        [Delete("/api/invoices/{invoiceId}")]
        Task DeleteAsync(string invoiceId);

        [Post("/api/invoices/pay")]
        Task PayInvoicesAsync([Body] PayInvoicesRequest model);

        [Post("/api/invoices/sum")]
        Task<decimal> GetSumToPayInvoicesAsync([Body] GetSumToPayInvoicesRequest model);

        [Post("/api/invoices/dispute/mark")]
        Task MarkDisputeAsync([Body] MarkInvoiceDisputeRequest model);

        [Post("/api/invoices/dispute/cancel")]
        Task CancelDisputeAsync([Body] CancelInvoiceDisputeRequest model);

        [Get("/api/invoices/dispute/{invoiceId}")]
        Task<InvoiceDisputeInfoResponse> GetInvoiceDisputeInfoAsync(string invoiceId);
    }
}
