using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
{
    /// <summary>
    /// Represents mark invoice dispute request
    /// </summary>
    public class MarkInvoiceDisputeRequest
    {
        /// <summary>
        /// The invoice id
        /// </summary>
        public string InvoiceId { get; set; }
        /// <summary>
        /// The reason
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// The employee id
        /// </summary>
        public string EmployeeId { get; set; }
    }
}
