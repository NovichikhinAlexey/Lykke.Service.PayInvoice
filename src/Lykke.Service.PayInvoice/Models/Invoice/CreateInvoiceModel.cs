using System;
using System.ComponentModel.DataAnnotations;
using Lykke.Service.PayInvoice.Validation;
using LykkePay.Common.Validation;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class CreateInvoiceModel
    {
        [Required]
        public string Number { get; set; }
        
        [Required]
        [GreaterThan(0)]
        public decimal Amount { get; set; }
        
        [Required]
        public string SettlementAssetId { get; set; }
        
        [Required]
        public string PaymentAssetId { get; set; }
        
        [Required]
        public string ClientName { get; set; }
        
        [Required]
        [Email]
        public string ClientEmail { get; set; }

        [Required]
        [Guid]
        public string EmployeeId { get; set; }

        [Required]
        [RowKey]
        public string MerchantId { get; set; }

        public DateTime DueDate { get; set; }

        public string Note { get; set; }

        public string BillingCategory { get; set; }

        public bool Dispute { get; set; }
    }
}
