using System.Collections.Generic;
using System.Globalization;
using Common;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Utils;

namespace Lykke.Service.PayInvoice.Services.Extensions
{
    public static class LogContextExtensions
    {
        public static IDictionary<string, string> ToContext(this IEmployee model)
        {
            return new Dictionary<string, string>()
                .ToContext(nameof(model.Id), model.Id)
                .ToContext(nameof(model.MerchantId), model.MerchantId)
                .ToContext(nameof(model.Email), model.Email.SanitizeEmail())
                .ToContext(nameof(model.FirstName), model.FirstName)
                .ToContext(nameof(model.LastName), model.LastName);
        }
        
        public static IDictionary<string, string> ToContext(this IInvoice model)
        {
            return new Dictionary<string, string>()
                .ToContext(nameof(model.Id), model.Id)
                .ToContext(nameof(model.Number), model.Number)
                .ToContext(nameof(model.ClientName), model.ClientName)
                .ToContext(nameof(model.ClientEmail), model.ClientEmail.SanitizeEmail())
                .ToContext(nameof(model.Amount), model.Amount.ToString(CultureInfo.InvariantCulture))
                .ToContext(nameof(model.DueDate), model.DueDate.ToString(CultureInfo.InvariantCulture))
                .ToContext(nameof(model.Status), model.Status.ToString())
                .ToContext(nameof(model.PaymentRequestId), model.PaymentRequestId)
                .ToContext(nameof(model.SettlementAssetId), model.SettlementAssetId)
                .ToContext(nameof(model.PaymentAssetId), model.PaymentAssetId)
                .ToContext(nameof(model.MerchantId), model.MerchantId)
                .ToContext(nameof(model.EmployeeId), model.EmployeeId)
                .ToContext(nameof(model.CreatedDate), model.CreatedDate.ToString(CultureInfo.InvariantCulture));
        }
    }
}