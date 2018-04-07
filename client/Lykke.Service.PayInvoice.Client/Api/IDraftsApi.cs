using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IDraftsApi
    {
        [Post("/api/drafts")]
        Task<InvoiceModel> CreateAsync([Body] CreateInvoiceModel model);

        [Put("/api/drafts")]
        Task<InvoiceModel> UpdateAsync([Body] UpdateInvoiceModel model);
    }
}
