using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Models;
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
        /// Returns invoice details.
        /// </summary>
        /// <returns>The invoice details.</returns>
        /// <response code="200">The invoice details.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpGet]
        [Route("invoices/{invoiceId}/details")]
        [SwaggerOperation("InvoicesGetDetails")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetDetailsAsync(string invoiceId)
        {
            try
            {
                IInvoiceDetails invoiceDetails = await _invoiceService.GetDetailsAsync(invoiceId);

                var model = Mapper.Map<InvoiceDetailsModel>(invoiceDetails);
                
                return Ok(model);
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(GetDetailsAsync),
                    invoiceId.ToContext(nameof(invoiceId)).ToJson(), exception);
                
                return NotFound(ErrorResponse.Create(exception.Message));
            }
            catch (InvalidOperationException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(GetDetailsAsync),
                    invoiceId.ToContext(nameof(invoiceId)).ToJson(), exception);
                
                return BadRequest(ErrorResponse.Create(exception.Message));
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(GetDetailsAsync),
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
            IEnumerable<IInvoice> invoices = await _invoiceService.GetAsync(merchantId);

            var model = Mapper.Map<List<InvoiceModel>>(invoices);

            return Ok(model);
        }

        /// <summary>
        /// Returns invoice.
        /// </summary>
        /// <returns>The invoice.</returns>
        /// <response code="200">The invoice.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpGet]
        [Route("merchants/{merchantId}/invoices/{invoiceId}")]
        [SwaggerOperation("InvoicesGet")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync(string merchantId, string invoiceId)
        {
            IInvoice invoice = await _invoiceService.GetAsync(merchantId, invoiceId);

            if (invoice == null)
                return NotFound();

            var model = Mapper.Map<InvoiceModel>(invoice);

            return Ok(model);
        }

        /// <summary>
        /// Creates draft invoice.
        /// </summary>
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
                return BadRequest(ErrorResponse.Create("Invalid model.", ModelState));

            var invoice = Mapper.Map<Invoice>(model);
            invoice.MerchantId = merchantId;

            IInvoice newInvoice = await _invoiceService.CreateDraftAsync(invoice);

            return Ok(Mapper.Map<InvoiceModel>(newInvoice));
        }

        /// <summary>
        /// Updates draft invoice.
        /// </summary>
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
                return BadRequest(ErrorResponse.Create("Invalid model.", ModelState));

            try
            {
                await _invoiceService.UpdateDraftAsync(Mapper.Map<Invoice>(model));
                
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
                return BadRequest(ErrorResponse.Create("Invalid model.", ModelState));

            IInvoice invoice = await _invoiceService.CreateAsync(Mapper.Map<Invoice>(model));

            return Ok(Mapper.Map<InvoiceModel>(invoice));
        }

        /// <summary>
        /// Updates an invoice.
        /// </summary>
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
                return BadRequest(ErrorResponse.Create("Invalid model.", ModelState));

            try
            {
                IInvoice invoice = await _invoiceService.CreateFromDraftAsync(Mapper.Map<Invoice>(model));
                
                return Ok(Mapper.Map<InvoiceModel>(invoice));
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
        /// Deletes the rule by specified id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="merchantId">The merchant id.</param>
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