using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Core.Services;
using Lykke.Pay.Service.Invoces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Pay.Service.Invoces.Controllers
{
    /// <summary>
    /// Controller for assets
    /// </summary>
    [Route("api/[controller]")]
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService<IInvoiceEntity> _service;

        public InvoicesController(IInvoiceService<IInvoiceEntity> service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<bool> SaveInvoice([FromBody]InvoiceEntity invoice)
        {
            return await _service.SaveInvoice(invoice);
        }

        [HttpGet]
        public async Task<List<IInvoiceEntity>> GetInvoices()
        {
            return await _service.GetInvoices();
        }

        [HttpGet("{invoiceId}")]
        public async Task<IInvoiceEntity> GetInvoice(string invoiceId)
        {
            return await _service.GetInvoice(invoiceId);
        }

        [HttpGet("{invoiceId}/delete")]
        public async Task DeleteInvoice(string invoiceId)
        {
            await _service.DeleteInvoice(invoiceId);
        }
    }

}