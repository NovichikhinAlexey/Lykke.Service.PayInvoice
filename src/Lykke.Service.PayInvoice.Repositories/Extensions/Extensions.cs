using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories.Extensions
{
    internal static class Extensions
    {
        public static string PropertyEqual(this string property, string value)
        {
            return TableQuery.GenerateFilterCondition(property, QueryComparisons.Equal, value);
        }

        public static string PropertyNotEqual(this string property, string value)
        {
            return TableQuery.GenerateFilterCondition(property, QueryComparisons.NotEqual, value);
        }

        public static string Or(this string filterA, string filterB)
        {
            return TableQuery.CombineFilters(filterA, TableOperators.Or, filterB);
        }

        public static string And(this string filterA, string filterB)
        {
            return TableQuery.CombineFilters(filterA, TableOperators.And, filterB);
        }

        public static string OrIfNotEmpty(this string filterA, string filterB)
        {
            return filterA.IsNotEmpty() ? filterA.Or(filterB) : filterB;
        }

        public static string AndIfNotEmpty(this string filterA, string filterB)
        {
            return filterA.IsNotEmpty() ? filterA.And(filterB) : filterB;
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}
