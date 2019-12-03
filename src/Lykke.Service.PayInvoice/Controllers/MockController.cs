using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.PayInvoice.Core.Domain.Notifications;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Models.Invoice;
using LykkePay.Common.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api/[controller]")]
    public class MockController : Controller
    {
        private readonly IInvoiceNotificationsService _notificationsService;

        public MockController(IInvoiceNotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        [HttpPost]
        [SwaggerOperation("MockPayment")]
        [ProducesResponseType(typeof(InvoiceModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task MockPayment([FromBody] InvoiceStatusUpdateNotification model)
        {
            await _notificationsService.NotifyStatusUpdateAsync(model);
        }
    }
}
