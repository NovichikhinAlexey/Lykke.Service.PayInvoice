using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain
{
    public class InvoiceFilter
    {
        public InvoiceFilter()
        {
            MerchantIds = new List<string>();
            ClientMerchantIds = new List<string>();
            Statuses = new List<InvoiceStatus>();
            BillingCategories = new List<string>();
        }

        public IEnumerable<string> MerchantIds { get; set; }
        public IEnumerable<string> ClientMerchantIds { get; set; }
        public IEnumerable<InvoiceStatus> Statuses { get; set; }
        public bool Dispute { get; set; }
        public IEnumerable<string> BillingCategories { get; set; }
        public decimal? GreaterThan { get; set; }
        public decimal? LessThan { get; set; }
    }
}
