using System.Collections.Generic;
using AutoMapper;
using Common;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Models.Employee;
using Lykke.Service.PayInvoice.Models.Invoice;

namespace Lykke.Service.PayInvoice.Extensions
{
    public static class ModelExtensions
    {
        public static string ToContext(this CreateEmployeeModel model)
        {
            var context = Mapper.Map<CreateEmployeeModel>(model);
            context.Email = context.Email?.SanitizeEmail();

            return context.ToJson();
        }

        public static string ToContext(this CreateInvoiceModel model)
        {
            var context = Mapper.Map<CreateInvoiceModel>(model);
            context.ClientEmail = context.ClientEmail?.SanitizeEmail();

            return context.ToJson();
        }

        public static string ToContext(this UpdateInvoiceModel model)
        {
            var context = Mapper.Map<UpdateInvoiceModel>(model);
            context.ClientEmail = context.ClientEmail?.SanitizeEmail();

            return context.ToJson();
        }

        public static IDictionary<string, string> ToContext(this UpdateEmployeeModel model)
        {
            return new Dictionary<string, string>()
                .ToContext(nameof(model.FirstName), model.Id)
                .ToContext(nameof(model.Email), model.Email.SanitizeEmail())
                .ToContext(nameof(model.FirstName), model.FirstName)
                .ToContext(nameof(model.LastName), model.LastName)
                .ToContext(nameof(model.LastName), model.MerchantId);
        }
    }
}