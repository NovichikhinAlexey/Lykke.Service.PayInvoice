using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Models.PaymentRequest;
using Lykke.Service.PayInvoice.Core;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.Email;
using Lykke.Service.PayInvoice.Core.Domain.Notifications;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Services.Extensions;

namespace Lykke.Service.PayInvoice.Services
{
    public class InvoiceNotificationsService : IInvoiceNotificationsService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IEmailService _emailService;
        private readonly IPayInternalClient _payInternalClient;

        public InvoiceNotificationsService(
            [NotNull] IEmailService emailService, 
            [NotNull] IInvoiceRepository invoiceRepository, 
            [NotNull] IPayInternalClient payInternalClient)
        {
            _emailService = emailService;
            _invoiceRepository = invoiceRepository;
            _payInternalClient = payInternalClient;
        }

        public async Task NotifyStatusUpdateAsync(InvoiceStatusUpdateNotification notification)
        {
            if (!notification.Status.IsPaidStatus()) return;

            Invoice invoice = await _invoiceRepository.FindByPaymentRequestIdAsync(notification.PaymentRequestId);

            if (invoice == null)
            {
                PaymentRequestModel paymentRequest = await _payInternalClient.GetPaymentRequestAsync(
                    notification.MerchantId,
                    notification.PaymentRequestId);

                if (paymentRequest.Initiator != Constants.PaymentRequestInitiator)
                    return;

                throw new InvoiceNotFoundException();
            }

            await _emailService.Send(new PaymentReceivedEmail
            {
                MerchantId = invoice.MerchantId,
                EmployeeId = invoice.EmployeeId,
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.Number,
                PaidAmount = notification.Amount,
                PaymentAsset = notification.AssetId,
                PaidDate = notification.Date,
                Payer = invoice.ClientName
            });
        }
    }
}
