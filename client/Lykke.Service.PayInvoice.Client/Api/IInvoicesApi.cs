using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IInvoicesApi
    {
        [Get("/api/merchants/{merchantId}/invoices")]
        Task<IEnumerable<InvoiceModel>> GetAllAsync(string merchantId);
        
        [Get("/api/merchants/{merchantId}/invoices/{invoiceId}")]
        Task<InvoiceModel> GetByIdAsync(string merchantId, string invoiceId);
        
        //[Post("/api/merchants/{merchantId}/invoices/drafts")]
        //Task<InvoiceModel> CreateDraftAsync(string merchantId, [Body] CreateDraftInvoiceModel model);

        //[Put("/api/merchants/{merchantId}/invoices/{invoiceId}")]
        //Task UpdateDraftAsync(string merchantId, string invoiceId, [Body] CreateDraftInvoiceModel model);

        //[Post("/api/merchants/{merchantId}/invoices")]
        //Task<InvoiceModel> CreateAsync(string merchantId, [Body] CreateInvoiceModel model);

        //[Post("/api/merchants/{merchantId}/invoices/{invoiceId}")]
        //Task<InvoiceModel> CreateFromDraftAsync(string merchantId, string invoiceId, [Body] CreateInvoiceModel model);
        
        [Delete("/api/merchants/{merchantId}/invoices/{invoiceId}")]
        Task DeleteAsync(string merchantId, string invoiceId);

        [Get("/api/invoices/{invoiceId}")]
        Task<InvoiceModel> GetAsync(string invoiceId);
        
        [Post("/api/invoices/{invoiceId}")]
        Task<InvoiceDetailsModel> CheckoutAsync(string invoiceId);

        [Post("/api/invoices/{invoiceId}/history")]
        Task<HistoryItemModel> GetHistoryAsync(string invoiceId);
    }
}
