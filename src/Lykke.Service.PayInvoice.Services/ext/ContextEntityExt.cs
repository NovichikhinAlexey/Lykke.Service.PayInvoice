using System.Collections.Generic;
using Common;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Services.ext
{
    public static class ContextEntityExt
    {
        public static Dictionary<string, object> ToContext(this IInvoice invoice)
        {
            var result = new Dictionary<string, object>();
            result["InvoiceId"] = invoice.InvoiceId;
            result["InvoiceNumber"] = invoice.InvoiceNumber;
            result["Amount"] = invoice.Amount;
            result["Currency"] = invoice.Currency;
            result["ClientId"] = invoice.ClientId;
            result["ClientName"] = invoice.ClientName;
            result["ClientUserId"] = invoice.ClientUserId;
            result["ClientEmail"] = invoice.ClientEmail.SanitizeEmail();
            result["DueDate"] = invoice.DueDate;
            result["Label"] = invoice.Label;
            result["Status"] = invoice.Status;
            result["WalletAddress"] = invoice.WalletAddress;
            result["StartDate"] = invoice.StartDate;
            result["Transaction"] = invoice.Transaction;
            result["MerchantId"] = invoice.MerchantId;
            return result;
        }
    }
}