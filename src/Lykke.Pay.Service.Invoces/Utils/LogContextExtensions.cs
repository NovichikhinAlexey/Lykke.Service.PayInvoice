using AutoMapper;
using Common;
using Lykke.Pay.Service.Invoces.Models.Invoice;

namespace Lykke.Pay.Service.Invoces.Utils
{
    public static class LogContextExtensions
    {
        public static string ToContext(this UpdateInvoiceModel model)
        {
            var context = Mapper.Map<UpdateInvoiceModel>(model);
            context.ClientEmail = context.ClientEmail.SanitizeEmail();

            return context.ToJson();
        }

        public static string ToContext(this UpdateDraftInvoiceModel model)
        {
            var context = Mapper.Map<UpdateDraftInvoiceModel>(model);
            context.ClientEmail = context.ClientEmail.SanitizeEmail();

            return context.ToJson();
        }
    }
}
