using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class CreateDraftInvoiceModel
    {
        public string Number { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string SettlementAssetId { get; set; }
        
        [Required]
        public string PaymentAssetId { get; set; }
        
        public string ClientName { get; set; }
        
        [EmailAddress]
        public string ClientEmail { get; set; }
        
        public string EmployeeId { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }

        public string Note { get; set; }
    }
}
