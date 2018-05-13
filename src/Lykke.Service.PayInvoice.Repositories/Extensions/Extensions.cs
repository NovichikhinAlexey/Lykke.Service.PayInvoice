using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.PayInvoice.Repositories.Extensions
{
    internal static class Extensions
    {
        public static string PropertyEqual(this string property, string value)
        {
            return TableQuery.GenerateFilterCondition(property, QueryComparisons.Equal, value);
        }

        public static string Or(this string filterA, string filterB)
        {
            return TableQuery.CombineFilters(filterA, TableOperators.Or, filterB);
        }
    }
}
