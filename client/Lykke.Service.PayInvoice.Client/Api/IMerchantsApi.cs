using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.Invoice;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IMerchantsApi
    {
        [Get("/api/merchants/{merchantId}/invoices")]
        Task<IEnumerable<InvoiceModel>> GetAllAsync(string merchantId);
    }
}
