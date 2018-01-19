using AutoMapper;
using Common;
using Lykke.Service.PayInvoice.Models.Invoice;

namespace Lykke.Service.PayInvoice.Utils
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
