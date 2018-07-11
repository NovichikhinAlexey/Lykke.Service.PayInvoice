using System;
using System.Collections.Generic;
using Lykke.Service.PayInvoice.Core.Domain;

namespace Lykke.Service.PayInvoice.Core.Extensions
{
    public static class EmployeeExtensions
    {
        public static string FullName(this Employee employee)
        {
            return $"{employee.FirstName} {employee.LastName}";
        }
    }
}
