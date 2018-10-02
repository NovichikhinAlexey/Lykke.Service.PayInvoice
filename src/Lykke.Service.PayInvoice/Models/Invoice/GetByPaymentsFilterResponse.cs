using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    /// <summary>
    /// Represents the respnse of invoices by payments filter
    /// </summary>
    public class GetByPaymentsFilterResponse
    {
        public GetByPaymentsFilterResponse()
        {
            Invoices = new List<InvoiceModel>();
        }

        /// <summary>
        /// The invoices
        /// </summary>
        public IReadOnlyList<InvoiceModel> Invoices { get; set; }
        
        /// <summary>
        /// The attribute of existance additional invoices
        /// </summary>
        public bool HasMoreInvoices { get; set; }
    }
}
