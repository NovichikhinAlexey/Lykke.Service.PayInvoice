using System.Collections.Generic;
using System.Linq;
using Lykke.Pay.Service.Invoces.Core.Utils;
using Lykke.SettingsReader;

namespace Lykke.Pay.Service.Invoces.Utils
{
    public class CallbackUrlFormatter : ICallbackUrlFormatter
    {
        private const string ApiPath = "callback/invoice";

        private readonly IReloadingManager<string> _settings;

        public CallbackUrlFormatter(IReloadingManager<string> settings)
        {
            _settings = settings;
        }

        public string GetProgressUrl(string invoiceId)
        {
            return Combine(_settings.CurrentValue, ApiPath, invoiceId, "progress");
        }

        public string GetSuccessUrl(string invoiceId)
        {
            return Combine(_settings.CurrentValue, ApiPath, invoiceId, "success");
        }

        public string GetErrorUrl(string invoiceId)
        {
            return Combine(_settings.CurrentValue, ApiPath, invoiceId, "error");
        }

        private static string Combine(params string[] args)
        {
            if (args == null || args.Length == 0)
                return string.Empty;

            IEnumerable<string> parts = args.Where(o => !string.IsNullOrEmpty(o))
                .Select(o => o.TrimStart('/').TrimEnd('/'));

            return string.Join("/", parts);
        }
    }
}
