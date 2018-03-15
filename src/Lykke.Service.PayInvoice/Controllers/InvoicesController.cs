using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Extensions;
using Lykke.Service.PayInvoice.Models.Invoice;
using Lykke.Service.PayInvoice.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api")]
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILog _log;

        public InvoicesController(IInvoiceService invoiceService, ILog log)
        {
            _invoiceService = invoiceService;
            _log = log;
        }

        /// <summary>
        /// Returns invoice by id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The invoice.</returns>
        /// <response code="200">The invoice.</response>
        [HttpGet]
        [Route("invoices/{invoiceId}")]
        [SwaggerOperation("InvoicesGet")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAsync(string invoiceId)
        {
            Invoice invoice = await _invoiceService.GetByIdAsync(invoiceId);

            var model = Mapper.Map<InvoiceModel>(invoice);

            return Ok(model);
        }
        
        /// <summary>
        /// Returns checkout invoice details.
        /// </summary>
        /// <returns>The invoice details.</returns>
        /// <response code="200">The invoice details.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpPost]
        [Route("invoices/{invoiceId}")]
        [SwaggerOperation("InvoicesGetDetails")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CheckoutAsync(string invoiceId)
        {
            try
            {
                InvoiceDetails invoiceDetails = await _invoiceService.CheckoutAsync(invoiceId);

                var model = Mapper.Map<InvoiceDetailsModel>(invoiceDetails);
                
                return Ok(model);
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(CheckoutAsync),
                    invoiceId.ToContext(nameof(invoiceId)).ToJson(), exception);
                
                return NotFound(ErrorResponse.Create(exception.Message));
            }
            catch (InvalidOperationException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(CheckoutAsync),
                    invoiceId.ToContext(nameof(invoiceId)).ToJson(), exception);
                
                return BadRequest(ErrorResponse.Create(exception.Message));
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(CheckoutAsync),
                    invoiceId.ToContext(nameof(invoiceId)).ToJson(), exception);

                throw;
            }
        }

        /// <summary>
        /// Returns invoices by merchant id.
        /// </summary>
        /// <returns>The collection of invoices.</returns>
        /// <response code="200">The collection of invoices.</response>
        [HttpGet]
        [Route("merchants/{merchantId}/invoices")]
        [SwaggerOperation("InvoicesGetAll")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByMerchantIdAsync(string merchantId)
        {
            IEnumerable<Invoice> invoices = await _invoiceService.GetAsync(merchantId);

            var model = Mapper.Map<List<InvoiceModel>>(invoices);

            return Ok(model);
        }

        /// <summary>
        /// Returns an invoice.
        /// </summary>
        /// <returns>The invoice.</returns>
        /// <response code="200">The invoice.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpGet]
        [Route("merchants/{merchantId}/invoices/{invoiceId}")]
        [SwaggerOperation("InvoicesGetById")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync(string merchantId, string invoiceId)
        {
            Invoice invoice = await _invoiceService.GetAsync(merchantId, invoiceId);

            if (invoice == null)
                return NotFound();

            var model = Mapper.Map<InvoiceModel>(invoice);

            return Ok(model);
        }

        /// <summary>
        /// Creates draft invoice.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="200">Created invoice.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [Route("merchants/{merchantId}/invoices/drafts")]
        [SwaggerOperation("InvoicesCreateDraft")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateDraftAsync(string merchantId, [FromBody]CreateDraftInvoiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            var invoice = Mapper.Map<Invoice>(model);
            invoice.MerchantId = merchantId;

            Invoice newInvoice = await _invoiceService.CreateDraftAsync(invoice);

            return Ok(Mapper.Map<InvoiceModel>(newInvoice));
        }

        /// <summary>
        /// Updates draft invoice.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="204">Invoice successfully updated.</response>
        /// <response code="400">Invalid model.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpPut]
        [Route("merchants/{merchantId}/invoices/{invoiceId}")]
        [SwaggerOperation("InvoicesUpdateDraft")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateDraftAsync(string merchantId, string invoiceId, [FromBody]CreateDraftInvoiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                var invoice = Mapper.Map<Invoice>(model);
                invoice.MerchantId = merchantId;
                invoice.Id = invoiceId;

                await _invoiceService.UpdateDraftAsync(invoice);
                
                return NoContent();
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(UpdateDraftAsync),
                    model.ToContext(), exception);

                return NotFound();
            }
            catch (InvalidOperationException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(UpdateDraftAsync),
                    model.ToContext(), exception);

                return BadRequest(ErrorResponse.Create(exception.Message));
            }
        }

        /// <summary>
        /// Creates an invoice.
        /// </summary>
        /// <param name="merchantId">The invoice id.</param>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="200">Created invoice.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [Route("merchants/{merchantId}/invoices")]
        [SwaggerOperation("InvoicesCreate")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAsync(string merchantId, [FromBody]CreateInvoiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            var invoice = Mapper.Map<Invoice>(model);
            invoice.MerchantId = merchantId;

            Invoice newInvoice = await _invoiceService.CreateAsync(invoice);

            return Ok(Mapper.Map<InvoiceModel>(newInvoice));
        }

        /// <summary>
        /// Updates an invoice.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="200">Created invoice.</response>
        /// <response code="400">Invalid model.</response>
        /// <response code="404">Draft invoice not found.</response>
        [HttpPost]
        [Route("merchants/{merchantId}/invoices/{invoiceId}")]
        [SwaggerOperation("InvoicesCreateFromDraft")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateFromDraftAsync(string merchantId, string invoiceId, [FromBody]CreateInvoiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                var invoice = Mapper.Map<Invoice>(model);
                invoice.MerchantId = merchantId;
                invoice.Id = invoiceId;

                Invoice newInvoice = await _invoiceService.CreateFromDraftAsync(invoice);
                
                return Ok(Mapper.Map<InvoiceModel>(newInvoice));
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(CreateFromDraftAsync),
                    model.ToContext(), exception);

                return NotFound();
            }
            catch (InvalidOperationException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(CreateFromDraftAsync),
                    model.ToContext(), exception);

                return BadRequest(ErrorResponse.Create(exception.Message));
            }
        }
        
        /// <summary>
        /// Deletes an invoice by specified id.
        /// </summary>
        /// <param name="merchantId">The merchant id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <response code="204">Invoice successfully deleted.</response>
        [HttpDelete]
        [Route("merchants/{merchantId}/invoices/{invoiceId}")]
        [SwaggerOperation("InvoicesDelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteAsync(string merchantId, string invoiceId)
        {
            await _invoiceService.DeleteAsync(merchantId, invoiceId);

            return NoContent();
        }
    }
}