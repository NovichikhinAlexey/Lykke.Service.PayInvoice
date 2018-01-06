using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Common;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Core.Services
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
        Task<Tuple<IInvoice, IOrder>> GetOrderDetails(string merchantId, string invoiceId);
        Task DeleteAsync(string merchantId, string invoiceId);
    }
}