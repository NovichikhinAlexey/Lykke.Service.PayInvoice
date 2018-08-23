using System;
using System.Runtime.CompilerServices;
using Common;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.Service.PayInvoice.Core.Extensions
{
    public static class LogExtensions
    {
        public static void InfoWithDetails(
            this ILog log,
            string message,
            object details,
            [CallerMemberName] string process = null)
        {
            log.Info(process, message, context: $"details: {details.ToJson()}");
        }

        public static void WarningWithDetails(
            this ILog log,
            string message,
            object details,
            [CallerMemberName] string process = null)
        {
            log.Warning(process, message, exception: null, context: $"details: {details.ToJson()}");
        }

        public static void ErrorWithDetails(
            this ILog log,
            Exception exception,
            object details,
            [CallerMemberName] string process = null)
        {
            log.Error(exception, exception?.Message, details, process);
        }

        public static void ErrorWithDetails(
            this ILog log,
            string message,
            object details,
            [CallerMemberName] string process = null)
        {
            log.Error(null, message, details, process);
        }

        public static void Error(
            this ILog log,
            Exception exception,
            string message,
            object details,
            string process)
        {
            log.Error(process, exception, message, context: $"details: {details.ToJson()}");
        }
    }
}
