using System;
using System.Globalization;

namespace Lykke.Service.PayInvoice.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class GreaterThanAttribute : UseWithRequiredAttribute
    {
        public GreaterThanAttribute(int mininum) => Minimum = mininum;

        public GreaterThanAttribute(decimal mininum) => Minimum = mininum;

        public decimal Minimum { get; set; }

        public override bool IsValid(object value)
        {
            if (IsRequiredResponsibility(value)) return true;

            if (value is int || value is int?)
            {
                return (int)value > Minimum;
            }

            if (value is decimal || value is decimal?)
            {
                return (decimal)value > Minimum;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The field {name} must be greater than {Minimum}.";
        }
    }
}
