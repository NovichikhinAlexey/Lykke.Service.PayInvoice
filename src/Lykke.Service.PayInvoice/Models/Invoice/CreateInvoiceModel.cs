using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class CreateInvoiceModel
    {
        [Required]
        public string Number { get; set; }
        
        [Required]
        public double Amount { get; set; }
        
        [Required]
        public string AssetId { get; set; }
        
        [Required]
        public string ExchangeAssetId { get; set; }
        
        [Required]
        public string ClientName { get; set; }
        
        [Required]
        public string ClientEmail { get; set; }
        
        public string EmployeeId { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }
    }
}
