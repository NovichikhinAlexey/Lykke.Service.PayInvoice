using AutoMapper;
using Common;
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
    }
}