using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    public class InvoiceFilter
    {
        public InvoiceFilter()
        {
            Statuses = new List<InvoiceStatus>();
            BillingCategories = new List<string>();
        }

        public string MerchantId { get; set; }
        public string ClientMerchantId { get; set; }
        public IEnumerable<InvoiceStatus> Statuses { get; set; }
        public bool Dispute { get; set; }
        public IEnumerable<string> BillingCategories { get; set; }
        public int? GreaterThan { get; set; }
        public int? LessThan { get; set; }
    }
}
