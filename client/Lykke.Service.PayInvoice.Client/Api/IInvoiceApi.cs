using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IInvoiceApi
    {
        [Get("/api/invoice/{invoiceId}/summary")]
        Task<InvoiceSummaryModel> GetSummaryAsync(string invoiceId);

        [Get("/api/invoice/merchant/{merchantId}/{invoiceId}")]
        Task<InvoiceModel> GetAsync(string merchantId, string invoiceId);

        [Get("/api/invoice/merchant/{merchantId}")]
        Task<IEnumerable<InvoiceModel>> GetAllAsync(string merchantId);

        [Post("/api/invoice/draft")]
        Task<InvoiceModel> CreateDraftAsync([Body] NewDraftInvoiceModel model);

        [Post("/api/invoice/draft/update")]
        Task UpdateDraftAsync([Body] UpdateDraftInvoiceModel model);

        [Post("/api/invoice/generate")]
        Task<InvoiceModel> GenerateAsync([Body] NewInvoiceModel model);

        [Post("/api/invoice/generate/draft")]
        Task<InvoiceModel> GenerateFromDraftAsync([Body] UpdateInvoiceModel model);

        [Delete("/api/invoice/{merchantId}/{invoiceId}")]
        Task DeleteAsync(string merchantId, string invoiceId);
    }
}
