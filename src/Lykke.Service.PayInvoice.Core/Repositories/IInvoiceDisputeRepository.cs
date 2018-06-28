using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IInvoiceDisputeRepository
    {
        Task<InvoiceDispute> GetByInvoiceIdAsync(string invoiceId);
        Task InsertAsync(InvoiceDispute invoiceDispute);
    }
}
