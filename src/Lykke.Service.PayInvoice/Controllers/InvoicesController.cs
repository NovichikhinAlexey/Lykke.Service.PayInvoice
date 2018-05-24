using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Extensions;
using Lykke.Service.PayInvoice.Models.Invoice;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
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
        /// <response code="404">Invoice not found.</response>
        [HttpGet]
        [Route("{invoiceId}")]
        [SwaggerOperation("InvoicesGet")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync(string invoiceId)
        {
            Invoice invoice = await _invoiceService.GetByIdAsync(invoiceId);

            if (invoice == null)
                return NotFound();

            var model = Mapper.Map<InvoiceModel>(invoice);

            return Ok(model);
        }

        /// <summary>
        /// Creates an invoice.
        /// </summary>
        /// <param name="model">The model that describe an invoice.</param>
        /// <response code="200">Created invoice.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [SwaggerOperation("InvoicesCreate")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody]CreateInvoiceModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            Invoice invoice = await _invoiceService.CreateAsync(Mapper.Map<Invoice>(model));

            return Ok(Mapper.Map<InvoiceModel>(invoice));
        }

        /// <summary>
        /// Create invoice by existing draft.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <response code="200">Created invoice.</response>
        /// <response code="404">Draft invoice not found.</response>
        [HttpPost]
        [Route("{invoiceId}")]
        [SwaggerOperation("InvoicesCreateFromDraft")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateFromDraftAsync(string invoiceId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse().AddErrors(ModelState));

            try
            {
                Invoice invoice = await _invoiceService.CreateFromDraftAsync(invoiceId);
                
                return Ok(Mapper.Map<InvoiceModel>(invoice));
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(CreateFromDraftAsync),
                    new {invoiceId}.ToJson(), exception);

                return NotFound();
            }
            catch (InvalidOperationException exception)
            {
                await _log.WriteErrorAsync(nameof(InvoicesController), nameof(CreateFromDraftAsync),
                    new {invoiceId}.ToJson(), exception);

                return BadRequest(ErrorResponse.Create(exception.Message));
            }
        }

        /// <summary>
        /// Deletes an invoice by specified id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <response code="204">Invoice successfully deleted.</response>
        /// <response code="400">Problem occured.</response>
        [HttpDelete]
        [Route("{invoiceId}")]
        [SwaggerOperation("InvoicesDelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAsync(string invoiceId)
        {
            try
            {
                await _invoiceService.DeleteAsync(invoiceId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ErrorResponse.Create(ex.Message));
            }

            return NoContent();
        }

        /// <summary>
        /// Returns invoice history.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>A collection of invoice history items.</returns>
        /// <response code="200">A collection of invoice history items.</response>
        [HttpGet]
        [Route("{invoiceId}/history")]
        [SwaggerOperation("InvoicesGetHistory")]
        [ProducesResponseType(typeof(IEnumerable<HistoryItemModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHistoryAsync(string invoiceId)
        {
            IReadOnlyList<HistoryItem> history = await _invoiceService.GetHistoryAsync(invoiceId);

            var model = Mapper.Map<List<HistoryItemModel>>(history);

            return Ok(model);
        }

        /// <summary>
        /// Returns invoices by filter
        /// </summary>
        /// <param name="merchantIds">The merchant ids (e.g. ?merchantIds=one&amp;merchantIds=two)</param>
        /// <param name="clientMerchantIds">The merchant ids of the clients (e.g. ?clientMerchantIds=one&amp;clientMerchantIds=two)</param>
        /// <param name="statuses">The statuses (e.g. ?statuses=one&amp;statuses=two)</param>
        /// <param name="dispute">The dispute attribute</param>
        /// <param name="billingCategories">The billing categories (e.g. ?billingCategories=one&amp;billingCategories=two)</param>
        /// <param name="greaterThan">The greater than number for filtering</param>
        /// <param name="lessThan">The less than number for filtering</param>
        /// <response code="200">A collection of invoices.</response>
        /// <response code="400">Problem occured.</response>
        [HttpGet]
        [Route("filter")]
        [SwaggerOperation("InvoicesGetByFilter")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByFilter(IEnumerable<string> merchantIds, IEnumerable<string> clientMerchantIds, IEnumerable<string> statuses, bool? dispute, IEnumerable<string> billingCategories, int? greaterThan, int? lessThan)
        {
            var statusesConverted = new List<InvoiceStatus>();

            if (statuses != null)
            {
                foreach (var status in statuses)
                {
                    try
                    {
                        statusesConverted.Add(status.Trim().ParseEnum<InvoiceStatus>());
                    }
                    catch (Exception)
                    {
                        return BadRequest(ErrorResponse.Create($"Invoice status <{status}> is not valid"));
                    }
                }
            }

            IReadOnlyList<Invoice> invoices = await _invoiceService.GetByFilterAsync(new InvoiceFilter
            {
                MerchantIds = merchantIds ?? new List<string>(),
                ClientMerchantIds = clientMerchantIds ?? new List<string>(),
                Statuses = statusesConverted,
                Dispute = dispute ?? false,
                BillingCategories = billingCategories ?? new List<string>(),
                GreaterThan = greaterThan,
                LessThan = lessThan
            });

            var model = Mapper.Map<List<InvoiceModel>>(invoices);

            return Ok(model);
        }
    }
}