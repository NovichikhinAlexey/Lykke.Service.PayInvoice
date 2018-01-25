﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class CreateDraftInvoiceModel
    {
        public string Number { get; set; }
        
        [Required]
        public double Amount { get; set; }
        
        [Required]
        public string AssetId { get; set; }
        
        [Required]
        public string ExchangeAssetId { get; set; }
        
        public string ClientName { get; set; }
        
        [EmailAddress]
        public string ClientEmail { get; set; }
        
        public string EmployeeId { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }
    }
}
