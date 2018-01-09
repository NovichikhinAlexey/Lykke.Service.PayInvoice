using System.Net;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Pay.Common;
using Lykke.Pay.Service.Invoces.Core.Exceptions;
using Lykke.Pay.Service.Invoces.Core.Services;
using Lykke.Pay.Service.Invoces.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Pay.Service.Invoces.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CallbackController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILog _log;

        public CallbackController(IInvoiceService invoiceService, ILog log)
        {
            _invoiceService = invoiceService;
            _log = log;
        }

        /// <summary>
        /// Updates invoice status.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="response">The model that describe a payment response.</param>
        /// <response code="204">Invoice status updated.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpPost]
        [Route("invoice/{invoiceId}/success")]
        [SwaggerOperation("CallbackSuccess")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SuccessAsync(string invoiceId, [FromBody]PaymentSuccessReturn response)
        {
            await _log.WriteInfoAsync(nameof(CallbackController), nameof(SuccessAsync),
                invoiceId.ToContext(nameof(invoiceId)).ToJson(), response?.ToJson());

            try
            {
                await _invoiceService.UpdateStatus(invoiceId, InvoiceStatus.Paid);
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteWarningAsync(nameof(CallbackController), nameof(SuccessAsync),
                    invoiceId.ToContext(nameof(invoiceId)).ToJson(), exception.Message);

                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Updates invoice status.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="response">The model that describe a payment response.</param>
        /// <response code="204">Invoice status updated.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpPost]
        [Route("invoice/{invoiceId}/progress")]
        [SwaggerOperation("CallbackProgress")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ProgressAsync(string invoiceId, [FromBody]PaymentInProgressReturn response)
        {
            await _log.WriteInfoAsync(nameof(CallbackController), nameof(ProgressAsync),
                invoiceId.ToContext(nameof(invoiceId)).ToJson(), response?.ToJson());

            try
            {
                await _invoiceService.UpdateStatus(invoiceId, InvoiceStatus.InProgress);
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteWarningAsync(nameof(CallbackController), nameof(ProgressAsync),
                    invoiceId.ToContext(nameof(invoiceId)).ToJson(), exception.Message);

                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Updates invoice status.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="response">The model that describe a payment response.</param>
        /// <response code="204">Invoice status updated.</response>
        /// <response code="404">Invoice not found.</response>
        [HttpPost]
        [Route("invoice/{invoiceId}/error")]
        [SwaggerOperation("CallbackError")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ErrorAsync(string invoiceId, [FromBody]PaymentErrorReturn response)
        {
            await _log.WriteInfoAsync(nameof(CallbackController), nameof(ErrorAsync),
                invoiceId.ToContext(nameof(invoiceId)).ToJson(), response.ToJson());

            InvoiceStatus status = InvoiceStatus.Removed;

            if (response != null)
            {
                switch (response.PaymentResponse.PaymentError)
                {
                    case PaymentError.AMOUNT_ABOVE:
                        status = InvoiceStatus.Overpaid;
                        break;
                    case PaymentError.AMOUNT_BELOW:
                        status = InvoiceStatus.Underpaid;
                        break;
                    case PaymentError.PAYMENT_EXPIRED:
                        status = InvoiceStatus.LatePaid;
                        break;
                }
            }

            try
            {
                await _invoiceService.UpdateStatus(invoiceId, status);
            }
            catch (InvoiceNotFoundException exception)
            {
                await _log.WriteWarningAsync(nameof(CallbackController), nameof(ErrorAsync),
                    invoiceId.ToContext(nameof(invoiceId)).ToJson(), exception.Message);

                return NotFound();
            }

            return NoContent();
        }
    }
}
