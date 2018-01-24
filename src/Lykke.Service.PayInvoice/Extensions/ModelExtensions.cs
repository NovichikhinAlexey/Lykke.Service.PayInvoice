using System.Collections.Generic;
using Common;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Models.Employee;

namespace Lykke.Service.PayInvoice.Extensions
{
    public static class ModelExtensions
    {
        public static IDictionary<string, string> ToContext(this CreateEmployeeModel model)
        {
            return new Dictionary<string, string>()
                .ToContext(nameof(model.Email), model.Email.SanitizeEmail())
                .ToContext(nameof(model.FirstName), model.FirstName)
                .ToContext(nameof(model.LastName), model.LastName);
        }
    }
}