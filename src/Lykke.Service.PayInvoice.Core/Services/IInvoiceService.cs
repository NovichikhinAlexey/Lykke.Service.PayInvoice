using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Common;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IInvoiceService
    {
        Task<IInvoice> GetAsync(string merchantId, string invoiceId);
        Task<IEnumerable<IInvoice>> GetAllAsync();
        Task<IEnumerable<IInvoice>> GetByMerchantIdAsync(string merchantId);
        Task<IInvoice> GetByAddressAsync(string address);
        Task<IInvoice> CreateDraftAsync(IInvoice invoice);
        Task UpdateDraftAsync(IInvoice invoice);
        Task<IInvoice> GenerateAsync(IInvoice invoice);
        Task<IInvoice> GenerateFromDraftAsync(IInvoice invoice);
        Task UpdateStatus(string invoiceId, InvoiceStatus status);
        Task<Tuple<IInvoice, IOrder>> GetOrderDetails(string invoiceId);
        Task DeleteAsync(string merchantId, string invoiceId);
    }
}