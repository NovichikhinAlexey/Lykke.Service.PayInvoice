using System.ComponentModel.DataAnnotations;

namespace Lykke.Pay.Service.Invoces.Models.Invoice
{
    public class NewDraftInvoiceModel
    {
        [Required]
        public string MerchantId { get; set; }
        public string InvoiceNumber { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        [Required]
        public string DueDate { get; set; }
    }
}
