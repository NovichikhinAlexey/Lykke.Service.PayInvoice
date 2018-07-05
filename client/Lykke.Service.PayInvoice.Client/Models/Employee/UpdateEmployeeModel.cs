using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Client.Models.Employee
{
    public class UpdateEmployeeModel : CreateEmployeeModel
    {
        /// <summary>
        /// Get or set Employee Id
        /// </summary>
        public string Id { get; set; }
    }
}
