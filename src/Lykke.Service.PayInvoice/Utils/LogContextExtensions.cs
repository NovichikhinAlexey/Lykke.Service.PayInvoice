using AutoMapper;
using Common;
using Lykke.Service.PayInvoice.Models.Invoice;

namespace Lykke.Service.PayInvoice.Utils
{
    public static class LogContextExtensions
    {
        public static string ToContext(this CreateInvoiceModel model)
        {
            var context = Mapper.Map<CreateInvoiceModel>(model);
            context.ClientEmail = context.ClientEmail?.SanitizeEmail();

            return context.ToJson();
        }

        public static string ToContext(this CreateDraftInvoiceModel model)
        {
            var context = Mapper.Map<CreateDraftInvoiceModel>(model);
            context.ClientEmail = context.ClientEmail?.SanitizeEmail();

            return context.ToJson();
        }
    }
}
