using System.Collections.Generic;

namespace Lykke.Pay.Service.Invoces.Core.Utils
{
    public static class LogContextUtility
    {
        public static IDictionary<string, string> ToContext(this string value, string key)
        {
            return new Dictionary<string, string> {{key, value}};
        }

        public static IDictionary<string, object> ToContext(this Dictionary<string, object> context, string key, object value)
        {
            context[key] = value;
            return context;
        }
    }
}
