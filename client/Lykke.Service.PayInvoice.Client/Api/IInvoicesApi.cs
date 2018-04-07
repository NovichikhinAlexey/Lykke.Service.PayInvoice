using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IInvoicesApi
    {
        [Get("/api/invoices/{invoiceId}")]
        Task<InvoiceModel> GetAsync(string invoiceId);

        [Get("/api/invoices/{invoiceId}/history")]
        Task<IReadOnlyList<HistoryItemModel>> GetHistoryAsync(string invoiceId);

        [Post("/api/invoices")]
        Task<InvoiceModel> CreateAsync([Body] CreateInvoiceModel model);

        [Post("/api/invoices/{invoiceId}")]
        Task<InvoiceModel> CreateFromDraftAsync(string invoiceId);
        
        [Delete("/api/invoices/{invoiceId}")]
        Task DeleteAsync(string invoiceId);
    }
}
