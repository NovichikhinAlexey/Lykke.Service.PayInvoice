using Common;
using Lykke.Service.PayInvoice.Core.Domain;

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

        public static Employee SanitizeCopy(this Employee model)
        {
            return model.ShallowCopy().Sanitize();
        }

        public static Invoice Sanitize(this Invoice model)
        {
            model.ClientEmail = model.ClientEmail.SanitizeEmail();
            return model;
        }

        public static Invoice SanitizeCopy(this Invoice model)
        {
            return model.ShallowCopy().Sanitize();
        }

        public static string Sanitize(this string str)
        {
            str = str.SanitizeEmail();
            return str;
        }
    }
}
