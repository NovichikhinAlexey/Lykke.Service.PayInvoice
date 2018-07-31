using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Extensions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Extensions;
using Lykke.Service.PayInvoice.Models.Invoice;
using Lykke.Service.PayInvoice.Validation;
using LykkePay.Common.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IMerchantService _merchantService;
        private readonly IMerchantSettingService _merchantSettingService;
        private readonly IEmployeeService _employeeService;
        private readonly ILog _log;

        public InvoicesController(
            IInvoiceService invoiceService,
            IMerchantService merchantService,
            IMerchantSettingService merchantSettingService,
            IEmployeeService employeeService,
            ILogFactory logFactory)
        {
            _invoiceService = invoiceService;
            _merchantService = merchantService;
            _merchantSettingService = merchantSettingService;
            _employeeService = employeeService;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Returns invoice by id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The invoice.</returns>
        /// <response code="200">The invoice.</response>
        /// <response code="404">Invoice not found.</response>
        /// <response code="400">Invalid model.</response>
        [HttpGet]
        [Route("{invoiceId}")]
        [SwaggerOperation("InvoicesGet")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> GetAsync([Required][Guid] string invoiceId)
        {
            try
            {
                Invoice invoice = await _invoiceService.GetByIdAsync(invoiceId);

                var model = Mapper.Map<InvoiceModel>(invoice);

                return Ok(model);
            }
            catch (InvoiceNotFoundException ex)
            {
                _log.ErrorWithDetails(ex, new { invoiceId });

                return NotFound(ErrorResponse.Create(ex.Message));
            }
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
        [ValidateModel]
        public async Task<IActionResult> CreateAsync([FromBody]CreateInvoiceModel model)
        {
            try
            {
                model.ValidateDueDate();

                Invoice invoice = await _invoiceService.CreateAsync(Mapper.Map<Invoice>(model));

                return Ok(Mapper.Map<InvoiceModel>(invoice));
            }
            catch (InvoiceDueDateException ex)
            {
                _log.WarningWithDetails(ex.Message, model.Sanitize());

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _log.ErrorWithDetails(ex, model.Sanitize());

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Create invoice by existing draft.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <response code="200">Created invoice.</response>
        /// <response code="404">Draft invoice not found.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [Route("{invoiceId}")]
        [SwaggerOperation("InvoicesCreateFromDraft")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ValidateModel]
        public async Task<IActionResult> CreateFromDraftAsync([Required][Guid] string invoiceId)
        {
            try
            {
                Invoice invoice = await _invoiceService.CreateFromDraftAsync(invoiceId);
                
                return Ok(Mapper.Map<InvoiceModel>(invoice));
            }
            catch (InvoiceNotFoundException ex)
            {
                _log.ErrorWithDetails(ex, new { invoiceId });

                return NotFound(ErrorResponse.Create(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _log.ErrorWithDetails(ex, new { invoiceId });

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Change payment asset of the invoice by creating new payment request with new asset
        /// </summary>
        /// <param name="invoiceId">The invoice id</param>
        /// <param name="paymentAssetId">The payment asset id</param>
        /// <response code="200">Updated invoice</response>
        /// <response code="404">Invoice not found.</response>
        /// <response code="400">Invalid model</response>
        [HttpPost]
        [Route("{invoiceId}/{paymentAssetId}")]
        [SwaggerOperation("ChangePaymentAsset")]
        [ProducesResponseType(typeof(InvoiceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ValidateModel]
        public async Task<IActionResult> ChangePaymentAssetAsync([Required][Guid] string invoiceId, [Required] string paymentAssetId)
        {
            try
            {
                Invoice invoice = await _invoiceService.ChangePaymentRequestAsync(invoiceId, paymentAssetId);

                return Ok(Mapper.Map<InvoiceModel>(invoice));
            }
            catch (InvoiceNotFoundException ex)
            {
                _log.ErrorWithDetails(ex, new { invoiceId });

                return NotFound(ErrorResponse.Create(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _log.ErrorWithDetails(ex, new { invoiceId, paymentAssetId });

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Deletes an invoice by specified id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <response code="204">Invoice successfully deleted.</response>
        /// <response code="404">Invoice not found.</response>
        /// <response code="400">Problem occured.</response>
        [HttpDelete]
        [Route("{invoiceId}")]
        [SwaggerOperation("InvoicesDelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> DeleteAsync([Required][Guid] string invoiceId)
        {
            try
            {
                await _invoiceService.DeleteAsync(invoiceId);
            }
            catch (InvoiceNotFoundException ex)
            {
                _log.ErrorWithDetails(ex, new { invoiceId });

                return NotFound(ErrorResponse.Create(ex.Message));
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
        /// <response code="400">Invalid model</response>
        [HttpGet]
        [Route("{invoiceId}/history")]
        [SwaggerOperation("InvoicesGetHistory")]
        [ProducesResponseType(typeof(IEnumerable<HistoryItemModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> GetHistoryAsync([Required][Guid] string invoiceId)
        {
            IReadOnlyList<HistoryItem> history = await _invoiceService.GetHistoryAsync(invoiceId);

            var model = Mapper.Map<List<HistoryItemModel>>(history);

            return Ok(model);
        }

        /// <summary>
        /// Returns invoice's payment requests
        /// </summary>
        /// <param name="invoiceId">The invoice id</param>
        /// <returns>A collection of invoice's payment requests</returns>
        /// <response code="200">A collection of invoice's payment requests</response>
        /// <response code="404">Invoice not found.</response>
        /// <response code="400">Invalid model</response>
        [HttpGet]
        [Route("{invoiceId}/paymentrequests")]
        [SwaggerOperation("InvoicesGetPaymentRequests")]
        [ProducesResponseType(typeof(IEnumerable<PaymentRequestHistoryItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> GetPaymentRequestsAsync([Required][Guid] string invoiceId)
        {
            try
            {
                IReadOnlyList<PaymentRequestHistoryItem> paymentRequests = await _invoiceService.GetPaymentRequestsOfInvoiceAsync(invoiceId);

                return Ok(paymentRequests);
            }
            catch (InvoiceNotFoundException ex)
            {
                _log.ErrorWithDetails(ex, new { invoiceId });

                return NotFound(ErrorResponse.Create(ex.Message));
            }
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
        [ValidateModel]
        public async Task<IActionResult> GetByFilter([RowKey] IEnumerable<string> merchantIds, [RowKey] IEnumerable<string> clientMerchantIds, IEnumerable<string> statuses, bool? dispute, IEnumerable<string> billingCategories, decimal? greaterThan, decimal? lessThan)
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

        /// <summary>
        /// Pay one or multiple invoices with certain amount
        /// </summary>
        /// <param name="model">Invoices ids and amount to pay</param>
        /// <response code="200">Accepted for further processing</response>
        /// <response code="400">Problem occured</response>
        [HttpPost("pay")]
        [SwaggerOperation("PayInvoices")]
        [ValidateModel]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PayInvoices([FromBody] PayInvoicesRequest model)
        {
            var validationResult = await ValidateForPayingInvoicesAsync(model);

            if (validationResult.HasError)
                return validationResult.ActionResult;

            try
            {
                await _invoiceService.PayInvoicesAsync(validationResult.Employee, validationResult.Invoices, validationResult.AssetForPay, model.Amount);
            }
            catch (InvalidOperationException ex)
            {
                _log.ErrorWithDetails(ex, model);

                return BadRequest(ErrorResponse.Create(ex.Message));
            }

            return Accepted();
        }

        /// <summary>
        /// Get sum for paying invoices
        /// </summary>
        /// <param name="model">Request model</param>
        /// <response code="200">Sum for paying invoices</response>
        /// <response code="400">Problem occured</response>
        [HttpPost("sum")]
        [SwaggerOperation("GetSumToPayInvoices")]
        [ValidateModel]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetSumToPayInvoices([FromBody] GetSumToPayInvoicesRequest model)
        {
            var validationResult = await ValidateForPayingInvoicesAsync(model);

            if (validationResult.HasError)
                return validationResult.ActionResult;

            decimal sum;

            try
            {
                 sum = await _invoiceService.GetSumInAssetForPayAsync(validationResult.Employee.MerchantId, validationResult.Invoices, validationResult.AssetForPay);
            }
            catch (InvalidOperationException ex)
            {
                _log.ErrorWithDetails(ex, model);

                return BadRequest(ErrorResponse.Create(ex.Message));
            }

            return Ok(sum);
        }

        private async Task<(IReadOnlyList<Invoice> Invoices, string AssetForPay, Employee Employee, bool HasError, IActionResult ActionResult)> ValidateForPayingInvoicesAsync(GetSumToPayInvoicesRequest model)
        {
            IReadOnlyList<Invoice> invoices = null;
            string assetForPay = null;
            Employee employee = null;
            bool hasError = false;
            IActionResult actionResult = null;

            try
            {
                employee = await _employeeService.GetByIdAsync(model.EmployeeId);

                // choose the asset for pay
                if (string.IsNullOrEmpty(model.AssetForPay))
                {
                    string baseAssetId = await _merchantSettingService.GetBaseAssetAsync(employee.MerchantId);

                    if (string.IsNullOrEmpty(baseAssetId))
                        throw new InvalidOperationException("BaseAsset for merchant is not found");

                    assetForPay = baseAssetId;
                }
                else
                {
                    assetForPay = model.AssetForPay;
                }

                invoices = await _invoiceService.ValidateForPayingInvoicesAsync(employee.MerchantId, model.InvoicesIds, model.AssetForPay);
            }
            catch (Exception ex)
            {
                hasError = true;
                _log.ErrorWithDetails(ex, model);

                switch (ex)
                {
                    case EmployeeNotFoundException _:
                    case MerchantNotFoundException _:
                    case MerchantGroupNotFoundException _:
                    case InvoiceNotFoundException _:
                        actionResult = NotFound(ErrorResponse.Create(ex.Message));
                        break;
                    case InvoiceNotInsideGroupException _:
                    case MerchantNotInvoiceClientException _:
                    case AssetNotAvailableForMerchantException _:
                    case InvalidOperationException _:
                        actionResult = BadRequest(ErrorResponse.Create(ex.Message));
                        break;
                    default:
                        throw;
                }
            }

            return (invoices, assetForPay, employee, hasError, actionResult);
        }

        /// <summary>
        /// Mark dispute
        /// </summary>
        /// <param name="model">The model</param>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        /// <response code="400">Invalid model</response>
        [HttpPost]
        [Route("dispute/mark")]
        [SwaggerOperation(nameof(MarkDispute))]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> MarkDispute([FromBody] MarkInvoiceDisputeRequest model)
        {
            try
            {
                await _invoiceService.MarkDisputeAsync(model.InvoiceId, model.Reason, model.EmployeeId);

                return Ok();
            }
            catch (InvoiceNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, model);

                return NotFound(ErrorResponse.Create(ex.Message));
            }
            catch (EmployeeNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, model);

                return NotFound(ErrorResponse.Create(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _log.ErrorWithDetails(ex, model);

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Cancel dispute
        /// </summary>
        /// <param name="model">The model</param>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        /// <response code="400">Invalid model</response>
        [HttpPost]
        [Route("dispute/cancel")]
        [SwaggerOperation(nameof(CancelDispute))]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> CancelDispute([FromBody] CancelInvoiceDisputeRequest model)
        {
            try
            {
                await _invoiceService.CancelDisputeAsync(model.InvoiceId, model.EmployeeId);

                return Ok();
            }
            catch (InvoiceNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, model);

                return NotFound(ErrorResponse.Create(ex.Message));
            }
            catch (EmployeeNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, model);

                return NotFound(ErrorResponse.Create(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _log.ErrorWithDetails(ex, model);

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        /// <summary>
        /// Get invoice dispute information
        /// </summary>
        /// <param name="invoiceId">The invoice id</param>
        /// <response code="200">Invoice dispute information</response>
        /// <response code="404">Not found</response>
        /// <response code="400">Invalid model</response>
        [HttpGet]
        [Route("dispute/{invoiceId}")]
        [SwaggerOperation(nameof(GetInvoiceDisputeInfo))]
        [ProducesResponseType(typeof(InvoiceDispute), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> GetInvoiceDisputeInfo([Required][Guid] string invoiceId)
        {
            try
            {
                InvoiceDispute invoiceDispute = await _invoiceService.GetInvoiceDisputeAsync(invoiceId);
                
                return Ok(invoiceDispute);
            }
            catch (InvoiceDisputeNotFoundException ex)
            {
                _log.WarningWithDetails(ex.Message, new { invoiceId });

                return NotFound(ErrorResponse.Create(ex.Message));
            }
        }
    }
}
