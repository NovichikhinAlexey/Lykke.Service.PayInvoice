using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class InvoiceModel
    {
        [Required]
        public string InvoiceId { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientName { get; set; }
        [Required]
        public string ClientUserId { get; set; }
        [Required]
        public string ClientEmail { get; set; }
        [Required]
        public string DueDate { get; set; }
        [Required]
        public string Label { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string WalletAddress { get; set; }
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string Transaction { get; set; }
        [Required]
        public string MerchantId { get; set; }
    }
}
