using System.Collections.Generic;
using System.Threading;
using Common;
using Lykke.Pay.Service.Invoces.Core.Domain;

namespace Lykke.Pay.Service.Invoces.Services.ext
{
    public static class ContextEntityExt
    {
        public static Dictionary<string, object> ToContext(this IInvoiceEntity invoice)
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

        public static Dictionary<string, object> ToContext(this IFileMetaEntity entity)
        {
            var result = new Dictionary<string, object>();
            result["InvoiceId"] = entity.InvoiceId;
            result["FileId"] = entity.FileId;
            result["FileName"] = entity.FileName;
            result["FileMetaType"] = entity.FileMetaType;
            result["FileSize"] = entity.FileSize;
            return result;
        }

        public static Dictionary<string, object> AddParam(this Dictionary<string, object> context, string key, object value)
        {
            context[key] = value;
            return context;
        }

        public static Dictionary<string, object> EmptyContext()
        {
            return new Dictionary<string, object>();
        }
    }
}