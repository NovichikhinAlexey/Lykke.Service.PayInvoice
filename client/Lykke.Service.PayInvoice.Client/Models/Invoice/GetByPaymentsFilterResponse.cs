using System.Collections.Generic;

namespace Lykke.Service.PayInvoice.Client.Models.Invoice
    {
    /// <summary>
    /// Represents the respnse of invoices by payments filter
    /// </summary>
    public class GetByPaymentsFilterResponse
    {
        /// <summary>
        /// Constructor of the class
        /// </summary>
        public GetByPaymentsFilterResponse() => Invoices = new List<InvoiceModel>();

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
