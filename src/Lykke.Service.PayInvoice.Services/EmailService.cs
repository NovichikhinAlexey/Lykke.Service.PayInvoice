using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.Service.EmailPartnerRouter.Client;
using Lykke.Service.EmailPartnerRouter.Contracts;
using Lykke.Service.PayInvoice.Core;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.Email;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayMerchant.Client;
using Lykke.Service.PayMerchant.Client.Models;

namespace Lykke.Service.PayInvoice.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailPartnerRouterClient _emailPartnerRouterClient;
        private readonly IPayMerchantClient _payMerchantClient;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly string _payInvoicePortalUrl;

        private const string PaymentReceivedTemplate = "lykkepay_payment_received";

        public EmailService(
            [NotNull] IEmailPartnerRouterClient emailPartnerRouterClient, 
            [NotNull] IPayMerchantClient payMerchantClient, 
            [NotNull] IEmployeeRepository employeeRepository, 
            [NotNull] string payInvoicePortalUrl)
        {
            _emailPartnerRouterClient = emailPartnerRouterClient;
            _payMerchantClient = payMerchantClient;
            _employeeRepository = employeeRepository;
            _payInvoicePortalUrl = payInvoicePortalUrl;
        }

        public async Task Send(PaymentReceivedEmail emailDetails)
        {
            MerchantModel merchant = await _payMerchantClient.Api.GetByIdAsync(emailDetails.MerchantId);

            Employee employee = await _employeeRepository.GetByIdAsync(emailDetails.EmployeeId);

            var emails = new List<string> { employee.Email };
            if (!string.IsNullOrEmpty(merchant.Email))
                emails.Add(merchant.Email);

            string invoiceUrl = _payInvoicePortalUrl.AddLastSymbolIfNotExists('/') +
                                $"invoices/{emailDetails.InvoiceId}";

            await _emailPartnerRouterClient.Send(new SendEmailCommand
            {
                ApplicationId = Constants.EmailApplicationId,
                EmailAddresses = emails.ToArray(),
                Template = PaymentReceivedTemplate,
                Payload = new Dictionary<string, string>
                {
                    {"UserName", $"{employee.FirstName} {employee.LastName}"},
                    {"PaidAmount", emailDetails.PaidAmount.ToString(CultureInfo.InvariantCulture)},
                    {"PaidAmountAsset", emailDetails.PaymentAsset},
                    {"InvoiceNumber", emailDetails.InvoiceNumber},
                    {"Payer", emailDetails.Payer},
                    {"PaidDate", emailDetails.PaidDate.ToString(CultureInfo.InvariantCulture)},
                    {"InvoiceUrl", invoiceUrl},
                    {"Year", DateTime.Today.Year.ToString()}
                }
            });
        }
    }
}
