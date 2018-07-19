using System.Collections.Generic;
using AutoMapper;
using Common;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Models.Employee;
using Lykke.Service.PayInvoice.Models.Invoice;
using Lykke.Service.PayInvoice.Services.Extensions;

namespace Lykke.Service.PayInvoice.Extensions
{
    public static class ModelExtensions
    {
        public static CreateEmployeeModel Sanitize(this CreateEmployeeModel model)
        {
            var context = Mapper.Map<CreateEmployeeModel>(model);
            context.Email = context.Email?.SanitizeEmail();
            context.FirstName = context.FirstName?.Sanitize();
            context.LastName = context.LastName?.Sanitize();

            return context;
        }

        public static CreateInvoiceModel Sanitize(this CreateInvoiceModel model)
        {
            var context = Mapper.Map<CreateInvoiceModel>(model);
            context.ClientEmail = context.ClientEmail?.SanitizeEmail();
            context.ClientName = context.ClientName?.Sanitize();

            return context;
        }

        public static UpdateInvoiceModel Sanitize(this UpdateInvoiceModel model)
        {
            var context = Mapper.Map<UpdateInvoiceModel>(model);
            context.ClientEmail = context.ClientEmail?.SanitizeEmail();
            context.ClientName = context.ClientName?.Sanitize();

            return context;
        }

        public static UpdateEmployeeModel Sanitize(this UpdateEmployeeModel model)
        {
            var context = Mapper.Map<UpdateEmployeeModel>(model);
            context.Email = context.Email?.SanitizeEmail();
            context.FirstName = context.FirstName?.Sanitize();
            context.LastName = context.LastName?.Sanitize();

            return context;
        }
    }
}
