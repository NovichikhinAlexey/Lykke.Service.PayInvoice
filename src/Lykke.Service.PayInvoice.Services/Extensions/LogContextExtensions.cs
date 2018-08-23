using System.Collections.Generic;
using System.Globalization;
using Common;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Utils;

namespace Lykke.Service.PayInvoice.Services.Extensions
{
    public static class LogContextExtensions
    {
        public static Employee Sanitize(this Employee model)
        {
            model.Email = model.Email.SanitizeEmail();
            model.FirstName = model.FirstName.Sanitize();
            model.LastName = model.LastName.Sanitize();
            return model;
        }

        public static Invoice Sanitize(this Invoice model)
        {
            model.ClientEmail = model.ClientEmail.SanitizeEmail();
            return model;
        }

        public static string Sanitize(this string str)
        {
            str = str.SanitizeEmail();
            return str;
        }
    }
}
