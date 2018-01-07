using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Core.Exceptions;
using Lykke.Pay.Service.Invoces.Core.Services;
using Lykke.Pay.Service.Invoces.Core.Utils;
using Lykke.Pay.Service.Invoces.Models;
using Lykke.Pay.Service.Invoces.Models.Invoice;
using Lykke.Pay.Service.Invoces.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Pay.Service.Invoces.Controllers
{
    [Route("api/[controller]")]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILog _log;

        public InvoiceController(IInvoiceService invoiceService, ILog log)
        {
            _invoiceService = invoiceService;
            _log = log;
        }

        /// <summary>
        /// Returns invoice summary information included order details.
        /// </summary>
        /// <returns>The invoice.</returns>
        /// <response code="200">The invoice.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpGet]
        [Route("{invoiceId}/summary")]
        [SwaggerOperation("InvoiceGet")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetSummaryAsync(string invoiceId)
        {
            Tuple<IInvoice, IOrder> summary;

            try
            {
                summary = await _invoiceService.GetOrderDetails(invoiceId);
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(InvoiceController), nameof(GetSummaryAsync),
                    invoiceId.ToContext(nameof(invoiceId)).ToJson(), exception);

                return BadRequest(ErrorResponse.Create(exception.Message));
            }

            var model = new InvoiceSummaryModel();

            Mapper.Map(summary.Item1, model);
            Mapper.Map(summary.Item2, model);

            return Ok(model);
        }

        /// <summary>
        /// Returns invoice.
        /// </summary>
        /// <returns>The invoice.</returns>
        /// <response code="200">The invoice.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpGet]
        [Route("{invoiceId}/merchant/{merchantId}")]
        [SwaggerOperation("InvoiceGet")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync(string invoiceId, string merchantId)
        {
            IInvoice invoice = await _invoiceService.GetAsync(merchantId, invoiceId);

            if (invoice == null)
                return NotFound();

            var model = Mapper.Map<InvoiceModel>(invoice);

            return Ok(model);
        }

        /// <summary>
        /// Returns invoices by merchant id.
        /// </summary>
        /// <returns>The collection of invoices.</returns>
        /// <response code="200">The collection of invoices.</response>
        [HttpGet]
        [Route("merchant/{merchantId}")]
        [SwaggerOperation("InvoiceGetByMerchantId")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByMerchantIdAsync(string merchantId)
        {
            IEnumerable<IInvoice> invoices = await _invoiceService.GetByMerchantIdAsync(merchantId);

            var model = Mapper.Map<List<InvoiceModel>>(invoices);

            return Ok(model);
        }

        /// <summary>
        /// Creates draft invoice.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="200">Generated invoice.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [Route("draft")]
        [SwaggerOperation("InvoiceCreateDraft")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateDraftAsync([FromBody]NewDraftInvoiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResponse.Create("Invalid model.", ModelState));

            IInvoice invoice = await _invoiceService.CreateDraftAsync(Mapper.Map<Invoice>(model));

            return Ok(Mapper.Map<InvoiceModel>(invoice));
        }

        /// <summary>
        /// Updates draft invoice.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="204">Generated invoice.</response>
        /// <response code="400">Invalid model.</response>
        /// <response code="404">Draft invoice not found.</response>
        [HttpPost]
        [Route("draft/update")]
        [SwaggerOperation("InvoiceUpdateDraft")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateDraftAsync([FromBody]UpdateDraftInvoiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResponse.Create("Invalid model.", ModelState));

            try
            {
                await _invoiceService.UpdateDraftAsync(Mapper.Map<Invoice>(model));
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoiceController), nameof(UpdateDraftAsync),
                    model.ToContext(), exception);

                return NotFound(ErrorResponse.Create(exception.Message));
            }
            catch (InvalidOperationException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoiceController), nameof(UpdateDraftAsync),
                    model.ToContext(), exception);

                return BadRequest(ErrorResponse.Create(exception.Message));
            }
            
            return NoContent();
        }

        /// <summary>
        /// Creates an invoice and order.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="200">Generated invoice.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [Route("generate")]
        [SwaggerOperation("InvoiceGenerate")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GenerateAsync([FromBody]NewInvoiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResponse.Create("Invalid model.", ModelState));

            IInvoice invoice = await _invoiceService.GenerateAsync(Mapper.Map<Invoice>(model));

            return Ok(Mapper.Map<InvoiceModel>(invoice));
        }

        /// <summary>
        /// Updates an invoice and create an order.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="200">Generated invoice.</response>
        /// <response code="400">Invalid model.</response>
        /// <response code="404">Draft invoice not found.</response>
        [HttpPost]
        [Route("generate/draft")]
        [SwaggerOperation("InvoiceGenerateFromDraft")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GenerateFromDraftAsync([FromBody]UpdateInvoiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorResponse.Create("Invalid model.", ModelState));

            IInvoice invoice;

            try
            {
                invoice = await _invoiceService.GenerateFromDraftAsync(Mapper.Map<Invoice>(model));
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoiceController), nameof(GenerateFromDraftAsync),
                    model.ToContext(), exception);

                return NotFound(ErrorResponse.Create(exception.Message));
            }
            catch (InvalidOperationException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoiceController), nameof(GenerateFromDraftAsync),
                    model.ToContext(), exception);

                return BadRequest(ErrorResponse.Create(exception.Message));
            }

            return Ok(Mapper.Map<InvoiceModel>(invoice));
        }
        
        /// <summary>
        /// Deletes the rule by specified id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="merchantId">The merchant id.</param>
        /// <response code="204">Invoice successfully deleted.</response>
        [HttpDelete]
        [Route("{invoiceId}/merchant/{merchantId}")]
        [SwaggerOperation("InvoiceDelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteAsync(string invoiceId, string merchantId)
        {
            await _invoiceService.DeleteAsync(merchantId, invoiceId);

            return NoContent();
        }
    }
}