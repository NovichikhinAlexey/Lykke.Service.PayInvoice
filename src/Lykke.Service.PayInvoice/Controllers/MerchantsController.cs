using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Models.Invoice;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class MerchantsController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public MerchantsController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Returns invoices by merchant id.
        /// </summary>
        /// <returns>The collection of invoices.</returns>
        /// <response code="200">The collection of invoices.</response>
        [HttpGet]
        [Route("{merchantId}/invoices")]
        [SwaggerOperation("MerchantsGetInvoices")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetGetInvoicesAsync(string merchantId)
        {
            IReadOnlyList<Invoice> invoices = await _invoiceService.GetAsync(merchantId);

            var model = Mapper.Map<List<InvoiceModel>>(invoices);

            return Ok(model);
        }
    }
}
