using System;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.Service.PayInvoice.Core.Extensions
{
    public static class LogExtensions
    {
        public static void Warning(
            this ILog log,
            string message,
            object context = null)
        {
            log.Warning(message, null, context);
        }

        public static void Error(
            this ILog log,
            Exception exception,
            object context = null)
        {
            log.Error(exception, null, context);
        }

        public static void Error(
            this ILog log,
            string message,
            object context = null)
        {
            log.Error(null, message, context);
        }

        public static void Error(
            this ILog log,
            string process,
            Exception exception = null,
            object context = null)
        {
            log.Error(process, exception, null, context);
        }
    }
}
