using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Client.Models.Invoice;
using Refit;

namespace Lykke.Pay.Service.Invoces.Client.Api
{
    internal interface IInvoiceApi
    {
        [Get("/api/invoice/{invoiceId}/merchant/{merchantId}")]
        Task<InvoiceModel> GetAsync(string invoiceId, string merchantId);

        [Get("/api/invoice/merchant/{merchantId}")]
        Task<IEnumerable<InvoiceModel>> GetByMerchantIdAsync(string merchantId);

        [Post("/api/invoice/draft")]
        Task<InvoiceModel> CreateDraftAsync([Body] NewInvoiceModel model);

        [Post("/api/invoice/draft/update")]
        Task UpdateDraftAsync([Body] InvoiceModel model);

        [Post("/api/invoice/generate")]
        Task<InvoiceModel> GenerateAsync([Body] NewInvoiceModel model);

        [Post("/api/invoice/generate/draft")]
        Task<InvoiceModel> GenerateFromDraftAsync([Body] InvoiceModel model);

        [Delete("/api/invoice/{invoiceId}/merchant/{merchantId}")]
        Task DeleteAsync(string invoiceId, string merchantId);
    }
}
