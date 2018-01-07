using System.ComponentModel.DataAnnotations;

namespace Lykke.Pay.Service.Invoces.Models.Invoice
{
    public class NewInvoiceModel
    {
        [Required]
        public string MerchantId { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string ClientName { get; set; }
        [Required]
        public string ClientEmail { get; set; }
        [Required]
        public string DueDate { get; set; }
    }
}
